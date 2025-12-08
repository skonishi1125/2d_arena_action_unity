using UnityEngine;

public class DarkKnight : Enemy
{
    public DarkKnightDashAttackState dashAttackState;
    public DarkKnightMeleeAttackState meleeAttackState;

    [Header("Melee Attack")]
    [SerializeField] public float meleeAttackDistance; // 近距離攻撃感知

    [Header("Dash Attack")]
    [SerializeField] public float dashAttackDistance; // ダッシュ攻撃感知距離
    [SerializeField] public float dashAttackXVelocity;// x加速度

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new DarkKnightBattleState(this, stateMachine, "battle");
        dashAttackState = new DarkKnightDashAttackState(this, stateMachine, "dashAttack");
        meleeAttackState = new DarkKnightMeleeAttackState(this, stateMachine, "meleeAttack");
        deadState = new EnemyDeadState(this, stateMachine, "NONE");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理
    }

    protected override void OnDrawBattleToAttackGizmos()
    {

        // 敵の中心から距離計測する
        // 違和感があれば、コメントアウトしたほうのGizmoの記載に戻す。
        Vector3 origin = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            origin,
            origin + Vector3.right * facingDir * dashAttackDistance
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            origin,
            origin + Vector3.right * facingDir * meleeAttackDistance
        );

        //// Battle -> DashAttack へと移行するための距離
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(
        //    playerCheck.position,
        //    new Vector3(playerCheck.position.x + (facingDir * dashAttackDistance), playerCheck.position.y)
        //);

        //// Battle -> melee へと移行するための距離
        //// 短いほうを後に書いたほうが上書きされるので、この順で書く
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(
        //    playerCheck.position,
        //    new Vector3(playerCheck.position.x + (facingDir * meleeAttackDistance), playerCheck.position.y)
        //);

    }

}
