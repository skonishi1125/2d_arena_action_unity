using UnityEngine;

public class PlayerGroundSlumFallState : PlayerState
{
    public PlayerGroundSlumFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.groundDetected)
            stateMachine.ChangeState(player.idleState);

    }
}
