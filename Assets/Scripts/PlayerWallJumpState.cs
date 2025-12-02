using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // x軸を向き(壁)と逆の方向に飛ばす
        player.SetVelocity(player.wallJumpDir.x * -player.facingDir, player.wallJumpDir.y);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (CanMultiJump())
            stateMachine.ChangeState(player.jumpState);

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);

        // ある程度自在に動けるようにしたい
        // 下記のようにすると、JumpStateが稼働するので別の対応が必要だが。
        //if (rb.linearVelocity.y > 0 && player.moveInput.x != 0)
        //    stateMachine.ChangeState(player.jumpState);

    }


}
