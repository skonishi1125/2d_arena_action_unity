using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(Player player, StateMachine stateMachine, string statename) : base(player, stateMachine, statename)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.jumpCount++;

        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);
    }


}
