using UnityEngine;

public class DemonGunner : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        // 遠距離攻撃
        rangeAttackState = new EnemyRangeAttackState(this, stateMachine, "attack");
        deadState = new EnemyDeadState(this, stateMachine, "NONE");

    }

    public override EnemyState GetNextAttackState()
    {
        return rangeAttackState;
    }
}
