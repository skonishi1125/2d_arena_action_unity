using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.idleState);
    }
}
