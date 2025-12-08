using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);

        // ダメージ倍率設定
        float dmgMul = enemy.commonAttackDamageMultiplier;
        enemy.EntityCombat.SetDamageMultiplier(dmgMul);

        // KB設定
        Vector2 kbPower = enemy.commonAttackKnockbackPower;
        enemy.EntityCombat.SetKnockback(kbPower, enemy.commonAttackKnockbackDuration);
    }
}
