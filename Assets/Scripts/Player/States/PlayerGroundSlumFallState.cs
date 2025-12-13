using UnityEngine;

public class PlayerGroundSlumFallState : PlayerState
{
    private float originalGravityScale;

    public PlayerGroundSlumFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0f, player.groundSlumFallForce);

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = player.groundSlumGravityScale;

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.groundDetected)
            stateMachine.ChangeState(player.groundSlumImpactState);

    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravityScale;
    }
}
