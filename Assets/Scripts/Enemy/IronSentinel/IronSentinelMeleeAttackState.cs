using UnityEngine;

public class IronSentinelMeleeAttackState : EnemyAttackState
{
    protected IronSentinel ironSentinel;
    public IronSentinelMeleeAttackState(IronSentinel ironSentinel, StateMachine stateMachine, string animBoolName) : base(ironSentinel, stateMachine, animBoolName)
    {
        this.ironSentinel = ironSentinel;
    }

    protected override void SetAttackDetail()
    {
        // 近接攻撃 ダメージ倍率設定
        float dmgMul = ironSentinel.meleeAttackDamageMultiplier;
        ironSentinel.EntityCombat.SetDamageMultiplier(dmgMul);

        // 近接攻撃 KB設定
        Vector2 kbPower = ironSentinel.meleeAttackKnockbackPower;
        ironSentinel.EntityCombat.SetKnockback(kbPower, ironSentinel.meleeAttackKnockbackDuration);
    }
}
