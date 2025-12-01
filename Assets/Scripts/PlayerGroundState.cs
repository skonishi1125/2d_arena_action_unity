using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // このStateを継承すれば、地上Stateの子状態のどれでも、
        // /ジャンプが押されたらジャンプに移行できるようになる
        if (input.Player.Jump.WasPerformedThisFrame())
            stateMachine.ChangeState(player.jumpState);

    }

}
