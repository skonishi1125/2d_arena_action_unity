using UnityEngine;

public class DemonHound : Enemy
{
    [Header("Attack Velocity")]
    [SerializeField] public float chargeAttackXVelocity = 2.5f;
    protected override void Awake()
    {
        base.Awake();

        // この割当はEnemy側ではやらない。
        // Enemy側で行うと、thisに入れる対象がどのモンスターなのか判断できなくなる
        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        // deadに入る時は、dead前のアニメーションを使う
        deadState = new EnemyDeadState(this, stateMachine, "NONE");

    }

}
