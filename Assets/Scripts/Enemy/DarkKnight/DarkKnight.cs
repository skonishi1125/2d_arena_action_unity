using UnityEngine;

public class DarkKnight : Enemy
{
    [SerializeField] public float stubAttackXVelocity = 10f;

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        deadState = new EnemyDeadState(this, stateMachine, "NONE");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理
    }
}
