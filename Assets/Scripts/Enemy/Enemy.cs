using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    private EnemyReward enemyReward;

    public EnemyIdleState idleState; // Playerと違い、さらに子要素で使うためpublic
    public EnemyMoveState moveState;
    public EnemyBattleState battleState;
    public EnemyAttackState attackState;
    public EnemyDeadState deadState;

    [Header("Battle Detail")]
    public float battleMoveSpeed = 3f; // battleState時のmove速度
    public float attackDistance = 2f; // 敵がAttack移行するために必要な距離
    [SerializeField] private float destroyWaitTime = 1f; // 敵が死んだときのDestroy時間

    // 移動速度などPlayer側と共有することもできるが、見やすくするため分割する
    [Header("Movement Details")]
    //待機時間 stateTimerと照らし合わせて使う
    // Player側のDashと同じ感じで、一定時間経過したらstateを移動するために使う。
    public float idleTime = 2f;
    public float moveSpeed = 2f;
    [Range(0,2)]
    public float moveAnimSpeedMultiplier = 1; // アニメーションスピード


    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer; // 感知距離のobjectがPlayerかどうか
    [SerializeField] private Transform playerCheck; // 感知用Raycastの始点
    [SerializeField] private float playerCheckDistance = 10f; // 感知距離

    // 攻撃されたときのplayer transform情報
    public Transform player {  get; private set; }


    protected override void Awake()
    {
        base.Awake();
        enemyReward = GetComponent<EnemyReward>();
    }
    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }


    // Playerから殴られたときなど、BattleStateに遷移させる為のメソッド
    public void TryEnterBattleState(Transform player)
    {
        // 既にbattlestate, attackstateの場合はスキップ
        if (stateMachine.currentState == battleState)
            return;

        if (stateMachine.currentState == attackState)
            return;

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    // Health側で死亡したとき、呼び出してdeadstateへ遷移できるようにする
    public override void Death()
    {
        base.Death();
        if (enemyReward != null)
            GameManager.Instance.playerLevel.AddExp(enemyReward.Exp);

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
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player")) {
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // GroundState -> Battle へと移行するための距離
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            playerCheck.position,
            new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y)
        );

        // Battle -> Attack へと移行するための距離
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            playerCheck.position,
            new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y)
        );

    }

}
