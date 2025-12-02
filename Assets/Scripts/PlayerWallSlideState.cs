public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        if (! player.wallDetected)
            stateMachine.ChangeState(player.fallState);

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
            player.Flip();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        HandleWallSlide();

    }


    // 張り付き中に下入力を受けた時、加速させる
    private void HandleWallSlide()
    {
        if (player.moveInput.y < 0)
        {
            // x方向に入力があったとき、壁と逆なら離れさせれば良い
            // moveInputが逆方向に入るとwallDetectedがfalseとなり、本Stateからは抜け出す
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
        }
        else
        {
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier);

        }
    }

}
