using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateMachine.currentState != this)
            return;

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState, "Fall: 地面着地");
            return;
        }

        if (player.wallDetected)
        {
            stateMachine.ChangeState(player.wallSlideState, "Fall: 壁にくっついた");
            return;
        }
    }
}
