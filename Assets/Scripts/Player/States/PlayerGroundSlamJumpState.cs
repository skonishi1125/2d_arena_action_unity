public class PlayerGroundSlamJumpState : PlayerState
{
    private float originalGravityScale;

    public PlayerGroundSlamJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        triggerCalled = false; // triggerの残留を防ぐ
        base.Enter();

        player.SetVelocity(
            rb.linearVelocity.x * player.groundSlamXMovementCompensation,
            player.groundSlamJumpForce
        );

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = player.groundSlamGravityScale;

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (rb.linearVelocity.y <= 0)
            stateMachine.ChangeState(player.groundSlamFallState);
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravityScale;
    }




}
