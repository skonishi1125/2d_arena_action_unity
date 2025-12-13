public class PlayerGroundSlumJumpState : PlayerState
{
    private float originalGravityScale;

    public PlayerGroundSlumJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(
            rb.linearVelocity.x * player.groundSlumXMovementCompensation,
            player.groundSlumJumpForce
        );

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = player.groundSlumGravityScale;

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (rb.linearVelocity.y <= 0)
            stateMachine.ChangeState(player.groundSlumFallState);
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravityScale;
    }




}
