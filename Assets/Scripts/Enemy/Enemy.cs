using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    private EnemyReward enemyReward;

    public EnemyIdleState idleState; // Playerと違い、さらに子要素で使うためpublic
    public EnemyMoveState moveState;
    public EnemyBattleState battleState;
    public EnemyAttackState attackState;
    public EnemyRangeAttackState rangeAttackState;
    public EnemyDeadState deadState;

    [SerializeField] private bool isBoss;

    [Header("Battle Detail")]
    public float battleMoveSpeed = 3f; // battleState時のmove速度
    public float attackDistance = 2f; // 敵がAttack移行するために必要な距離
    [SerializeField] private float destroyWaitTime = 1f; // 敵が死んだときのDestroy時間
    public float attackVerticalRange = 5f; // y軸にこの数値分離れた時、battle -> idleにする
    public float verticalOutOfRangeTime = 3f; // battle -> idleにする際のバッファ

    // 移動速度などPlayer側と共有することもできるが、見やすくするため分割する
    [Header("Movement Details")]
    //待機時間 stateTimerと照らし合わせて使う
    // Player側のDashと同じ感じで、一定時間経過したらstateを移動するために使う。
    public float idleTime = 2f;
    public float moveSpeed = 2f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1; // アニメーションスピード

    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer; // 感知距離のobjectがPlayerかどうか
    [SerializeField] protected Transform playerCheck; // 感知用Raycastの始点
    [SerializeField] protected float playerCheckDistance = 10f; // 感知距離

    [Header("Common Attack Details(non-boss)")]
    public float commonAttackDamageMultiplier;
    public Vector2 commonAttackKnockbackPower;
    public float commonAttackKnockbackDuration;

    // 攻撃されたときのplayer transform情報
    public Transform player { get; private set; }

    public bool IsBoss => isBoss;

    protected override void Awake()
    {
        base.Awake();
        enemyReward = GetComponent<EnemyReward>();

        // 設定されるべき値のチェック
        if (!LogHelper.AssertNotNull(playerCheck, nameof(playerCheck), this))
            return;

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理
    }



    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
            GameManager.Instance.Player.Health.OnDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
            GameManager.Instance.Player.Health.OnDied -= HandlePlayerDeath;
    }

    // BattleState から呼ばれ、次に遷移すべき攻撃ステートを返す。
    // デフォルトは近接 → 無ければ遠距離。
    // DemonGunnnerなど遠距離しか持たない敵は、
    // 固有クラス側でこのメソッドをOverrideして管理すればよい。
    public virtual EnemyState GetNextAttackState()
    {
        if (attackState != null)
            return attackState;

        if (rangeAttackState != null)
            return rangeAttackState;

        return null;
    }


    // Playerから殴られたときなど、BattleStateに遷移させる為のメソッド
    public void TryEnterBattleState(Transform player)
    {
        // 既にbattle, attack / rangeAttackの場合はスキップ
        if (stateMachine.currentState == battleState)
            return;

        if (stateMachine.currentState == attackState
            || stateMachine.currentState == rangeAttackState)
            return;

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    // Enemy死亡時の責務
    // * deadstateへ遷移できるようにする
    // * 経験値の付与
    // ※expbarの更新は、PlayerLevel, UIInGameの責務
    public override void Death()
    {
        base.Death();

        if (enemyReward != null)
            GameManager.Instance.Player.Level.AddExp(enemyReward.Exp);

        stateMachine.ChangeState(deadState);
    }

    public void DiedDestroy()
    {
        StartCoroutine(DiedDestroyCo());
    }

    private IEnumerator DiedDestroyCo()
    {
        yield return new WaitForSeconds(destroyWaitTime);
        Destroy(gameObject);
    }

    // Player側の死亡時のActionEventにsubscribeしているので、
    // Playerが倒れた時、BattleState / AttackStateなどからidleStateに戻る。
    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }



    // GroundStateで使うので、publicとする
    // 感知したPlayer | Groundの各種情報を返す。
    public RaycastHit2D PlayerDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround
        );

        // 検知なし & 検知したものがPlayerでない場合は、
        // 何も検知しなかった時のRaycastHit2Dクラスの形を返す
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return default;
        }

        return hit;

    }

    // playerのtransform情報を返す
    // 後ろから殴られた場合などは、TryEnterBattleState()でplayerを格納しているのでそのまま返す。
    // 感知から発見した場合は、感知対象のPlayerのtransform情報を返す。
    public Transform GetPlayerReference()
    {
        if (player == null)
            player = PlayerDetection().transform;

        return player;
    }

    // スポーン時などに初期向きを指定するためのヘルパ
    // 引数としてランダムに生成したdirなどを渡し、
    // その値に応じて決める。 +1（右） or -1（左）
    public void InitializeFacing(int dir)
    {
        if (dir > 0 && facingDir < 0)
            Flip();
        else if (dir < 0 && facingDir > 0)
            Flip();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        OnDrawBattleGizmos();
    }

    protected virtual void OnDrawBattleGizmos()
    {
        OnDrawGroundToBattleGizmos();
        OnDrawBattleToAttackGizmos();
        OnDrawVerticalToAttackGizmos();
    }

    protected virtual void OnDrawGroundToBattleGizmos()
    {
        // GroundState -> Battle へと移行するための距離
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            playerCheck.position,
            new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y)
        );
    }

    protected virtual void OnDrawBattleToAttackGizmos()
    {
        // Battle -> Attack へと移行するための距離
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            playerCheck.position,
            new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y)
        );
    }

    protected virtual void OnDrawVerticalToAttackGizmos()
    {
        // Battle -> idle へと移行するケースの縦の距離
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(0.2f, attackVerticalRange * 2, 0.2f)
        );
    }

}
