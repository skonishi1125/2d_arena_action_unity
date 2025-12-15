using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SetAttackDetail();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }

    // 通常攻撃のパラメータ設定
    // 例えばボスなど、MeleeAttackStateやDashAttackStateを持つ者がいる
    // その場合は、このメソッドをMeleeAttackState側でoverrideして、
    // Meleeの値やDashの値にすればよい
    protected virtual void SetAttackDetail()
    {
        // ダメージ倍率設定
        float dmgMul = enemy.commonAttackDamageMultiplier;
        enemy.EntityCombat.SetDamageMultiplier(dmgMul);

        // KB設定
        Vector2 kbPower = enemy.commonAttackKnockbackPower;
        enemy.EntityCombat.SetKnockback(kbPower, enemy.commonAttackKnockbackDuration);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.EntityCombat.ResetDamageMultiplier();
        enemy.EntityCombat.ResetKnockback();
    }

}
