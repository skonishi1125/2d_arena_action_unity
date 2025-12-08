using UnityEngine;

public class DarkKnightMeleeAttackState : EnemyAttackState
{
    protected DarkKnight darkKnight;

    public DarkKnightMeleeAttackState(DarkKnight darkKnight, StateMachine stateMachine, string animBoolName) : base(darkKnight, stateMachine, animBoolName)
    {
        this.darkKnight = darkKnight;
    }

    protected override void SetAttackDetail()
    {
        // 近接攻撃 ダメージ倍率設定
        float dmgMul = darkKnight.meleeAttackDamageMultiplier;
        darkKnight.EntityCombat.SetDamageMultiplier(dmgMul);

        // 近接攻撃 KB設定
        Vector2 kbPower = darkKnight.meleeAttackKnockbackPower;
        darkKnight.EntityCombat.SetKnockback(kbPower, darkKnight.meleeAttackKnockbackDuration);
    }

}
