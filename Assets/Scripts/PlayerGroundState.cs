using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.jumpCount = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (rb.linearVelocity.y < 0 && ! player.groundDetected)
            stateMachine.ChangeState(player.fallState);

        // このStateを継承すれば、地上Stateの子状態のどれでも、
        // 条件を満たせばジャンプに移行できるようになる
        if (CanMultiJump())
            stateMachine.ChangeState(player.jumpState);

        if (input.Player.Attack.WasPerformedThisFrame())
            stateMachine.ChangeState(player.basicAttackState);


    }

}
