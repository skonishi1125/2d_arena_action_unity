using UnityEngine;

public class PlayerIdleState : PlayerState
{
    // 親のEntityStateにコンストラクタが存在するので、同様に定義する
    public PlayerIdleState(Player player, StateMachine stateMachine, string statename) : base(player, stateMachine, statename)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0f, rb.linearVelocity.y); // 横滑り防止
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.moveInput.x != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
