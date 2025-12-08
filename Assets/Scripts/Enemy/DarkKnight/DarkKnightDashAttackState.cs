using UnityEngine;

public class DarkKnightDashAttackState : EnemyAttackState
{
    protected DarkKnight darkKnight;

    public DarkKnightDashAttackState(DarkKnight darkKnight, StateMachine stateMachine, string animBoolName) : base(darkKnight, stateMachine, animBoolName)
    {
        this.darkKnight = darkKnight;
    }

    protected override void SetAttackDetail()
    {
        // ダッシュ攻撃 ダメージ倍率設定
        float dmgMul = darkKnight.dashAttackDamageMultiplier;
        darkKnight.EntityCombat.SetDamageMultiplier(dmgMul);

        // ダッシュ攻撃 KB設定
        Vector2 kbPower = darkKnight.dashAttackKnockbackPower;
        darkKnight.EntityCombat.SetKnockback(kbPower, darkKnight.dashAttackKnockbackDuration);
    }

}
