public class PlayerGroundSlumJumpState : PlayerState
{
    public PlayerGroundSlumJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce * 2f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.groundSlumFallState);
    }




}
