using UnityEngine;

public class EnemyMoveState : EnemyGroundState
{
    public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // idleから再び動き出すとき、反転
        if (!enemy.groundDetected || enemy.wallDetected)
            enemy.Flip();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 床がない or 壁のとき、idleに
        if (! enemy.groundDetected || enemy.wallDetected)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
    }

}
