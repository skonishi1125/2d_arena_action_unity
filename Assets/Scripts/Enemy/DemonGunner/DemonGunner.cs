using UnityEngine;

public class DemonGunner : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        deadState = new EnemyDeadState(this, stateMachine, "NONE");

    }
}
