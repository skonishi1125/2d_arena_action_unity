using UnityEngine;

public class PlayerGroundSlumImpactState : PlayerState
{
    public PlayerGroundSlumImpactState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        // 仮で、空中攻撃の威力で殴ってみる
        // ダメージ倍率設定
        float dmgMul = player.airAttackDamageMultiplier;
        player.EntityCombat.SetDamageMultiplier(dmgMul);

        // KB設定
        Vector2 kbPower = player.airAttackKnockbackPower;
        player.EntityCombat.SetKnockback(kbPower, player.airAttackKnockbackDuration);

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        player.EntityCombat.ResetDamageMultiplier();
        player.EntityCombat.ResetKnockback();
    }

}
