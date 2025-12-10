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

        if(input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.airAttackState);

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // === 徐々にxを遅くしたいとき(一旦不採用)
        //float currentX = rb.linearVelocity.x;
        //float targetX = (player.moveInput.x != 0)
        //    ? player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier
        //    : 0f;
        //float smoothedX = Mathf.Lerp(currentX, targetX, 0.2f);

        // Playerが左右入力しているときだけ、移動させる
        //if (player.moveInput.x != 0)
        //{
        //    targetX = player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier;
        //} else
        //{
        //    targetX = 0f;
        //}

        // 空中でも左右入力が入ったら、その方向に移動
        // jump, fallどちらでも対応できるようにするため、SuperStateの本classに書く
        if (player.moveInput.x != 0)
        {
            float targetX = player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier;
            player.SetVelocity(targetX, rb.linearVelocity.y);
        }


    }

}
