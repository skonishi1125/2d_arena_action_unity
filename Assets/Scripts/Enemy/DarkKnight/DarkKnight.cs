using UnityEngine;

public class DarkKnight : Enemy
{
    public DarkKnightDashAttackState dashAttackState;

    [Header("Dash Attack")]
    [SerializeField] public float dashAttackDistance = 5f; // ダッシュ攻撃感知距離
    [SerializeField] public float dashAttackXVelocity = 10f;// x加速度

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");

        battleState = new DarkKnightBattleState(this, stateMachine, "battle");

        dashAttackState = new DarkKnightDashAttackState(this, stateMachine, "dashAttack");
        attackState = dashAttackState;   // ひとまず攻撃＝ダッシュ攻撃にしておく

        deadState = new EnemyDeadState(this, stateMachine, "NONE");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理
    }

    protected override void OnDrawBattleToAttackGizmos()
    {
        // Battle -> DashAttack へと移行するための距離
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            playerCheck.position,
            new Vector3(playerCheck.position.x + (facingDir * dashAttackDistance), playerCheck.position.y)
        );

    }

}
