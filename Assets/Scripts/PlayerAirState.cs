using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (CanMultiJump())
            stateMachine.ChangeState(player.jumpState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 空中でも左右入力が入ったら、その方向に移動
        // jump, fallどちらでも対応できるようにするため、super stateの本classに書く
        if (player.moveInput.x != 0)
            player.SetVelocity(
                player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier,
                rb.linearVelocity.y
            );

    }

}
