using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, StateMachine stateMachine, string statename) : base(player, stateMachine, statename)
    {
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 空中でも左右入力が入ったら、その方向に移動
        if (player.moveInput.x != 0)
            player.SetVelocity(
                player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier,
                rb.linearVelocity.y
            );

    }

}
