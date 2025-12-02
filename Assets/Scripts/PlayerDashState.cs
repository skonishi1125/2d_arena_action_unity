public class PlayerDashState : PlayerState
{
    // ダッシュ中は重力加速度を無効にする
    private float originalGravityScale;
    public PlayerDashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.dashDuration;
        originalGravityScale = rb.gravityScale; // 既存のrbを保存しておき、0にする
        rb.gravityScale = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateTimer < 0)
        {
            if (player.groundDetected)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.fallState);
            }

        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocity(player.dashSpeed * player.facingDir, 0);
    }

    public override void Exit()
    {
        base.Exit();
        // DashStateから出るとき、慣性と重力加速度を戻す
        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;
    }




}
