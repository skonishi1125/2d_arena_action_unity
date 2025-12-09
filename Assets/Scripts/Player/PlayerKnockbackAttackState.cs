using UnityEngine;

public class PlayerKnockbackAttackState : PlayerState
{
    public PlayerKnockbackAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // ダメージ倍率設定
        float dmgMul = player.knockbackAttackDamageMultiplier;
        player.EntityCombat.SetDamageMultiplier(dmgMul);
        // KB設定
        Vector2 kbPower = player.knockbackAttackKnockbackPower;
        player.EntityCombat.SetKnockback(kbPower, player.knockbackAttackKnockbackDuration);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 攻撃アニメ中に滑ってしまうのを防ぐ
        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit(); // これで anim.SetBool(false) を呼んでいる
        player.EntityCombat.ResetDamageMultiplier();
        player.EntityCombat.ResetKnockback();
    }
}
