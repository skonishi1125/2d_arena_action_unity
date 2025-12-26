using UnityEngine;

public class IronSentinel : Enemy
{
    // 2つ目の壁判定
    [SerializeField] private Transform secondWallCheck;

    public IronSentinelDashAttackState dashAttackState;
    public IronSentinelMeleeAttackState meleeAttackState;

    [Header("Melee Attack")]
    [SerializeField] public float meleeAttackDistance; // 近距離攻撃感知
    public float meleeAttackDamageMultiplier;
    public Vector2 meleeAttackKnockbackPower;
    public float meleeAttackKnockbackDuration;

    [Header("Dash Attack")]
    [SerializeField] public float dashAttackDistance; // ダッシュ攻撃感知距離
    [SerializeField] public float dashAttackXVelocity;// x加速度
    public float dashAttackDamageMultiplier;
    public Vector2 dashAttackKnockbackPower;
    public float dashAttackKnockbackDuration;

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new IronSentinelBattleState(this, stateMachine, "battle");
        dashAttackState = new IronSentinelDashAttackState(this, stateMachine, "dashAttack");
        meleeAttackState = new IronSentinelMeleeAttackState(this, stateMachine, "meleeAttack");
        deadState = new EnemyDeadState(this, stateMachine, "NONE");

    }

    protected override void HandleCollisionDetection()
    {
        base.HandleCollisionDetection();
        wallDetected = Physics2D.Raycast(
            secondWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround
        );
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // 壁
        Gizmos.DrawLine(
            secondWallCheck.position,
            secondWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0)
        );
    }

    protected override void OnDrawBattleToAttackGizmos()
    {

        // 敵の中心から距離計測する
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

    }

}
