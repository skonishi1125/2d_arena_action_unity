public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player, StateMachine stateMachine, string statename) : base(player, stateMachine, statename)
    {
    }


    // 入力を受け付け、移動できるようにする
    public override void Update()
    {
        base.Update();

        if (player.moveInput.x == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // todo: rb.linearVelocityでアクセスできるはずだが...
        player.SetVelocity(player.moveInput.x * player.moveSpeed, player.rb.linearVelocity.y);

    }
}
