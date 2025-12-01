public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player player, StateMachine stateMachine, string statename) : base(player, stateMachine, statename)
    {
    }

    // 入力を受け付け、移動できるようにする
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.moveInput.x == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
    }

}
