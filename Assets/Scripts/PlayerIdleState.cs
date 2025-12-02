using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    // 親のEntityStateにコンストラクタが存在するので、同様に定義する
    public PlayerIdleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0f, rb.linearVelocity.y); // 横滑り防止
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 向いている方向に進行しようとして、壁があるときはstate移行しない
        if (player.moveInput.x == player.facingDir && player.wallDetected)
            return;

        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
