using UnityEngine;

public class EnemyIdleState : EnemyGroundState
{
    public EnemyIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }


}
