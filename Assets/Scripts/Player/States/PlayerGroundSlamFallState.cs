using UnityEngine;

public class PlayerGroundSlamFallState : PlayerState
{
    private float originalGravityScale;

    public PlayerGroundSlamFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0f, player.groundSlamFallForce);

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = player.groundSlamGravityScale;

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.groundDetected)
            stateMachine.ChangeState(player.groundSlamImpactState);

    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravityScale;
    }
}
