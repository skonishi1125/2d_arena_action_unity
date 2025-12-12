using UnityEngine;

public class DemonGunner : Enemy
{
    public ProjectileDamageContext PendingProjectileCtx { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        // 遠距離攻撃
        rangeAttackState = new EnemyRangeAttackState(this, stateMachine, "attack");
        deadState = new EnemyDeadState(this, stateMachine, "NONE");

        // 弾技の初期設定
        PendingProjectileCtx = new ProjectileDamageContext
        {
            damageMultiplier = this.commonAttackDamageMultiplier,
            hasCustomKnockback = true,
            knockbackPower = this.commonAttackKnockbackPower,
            knockbackDuration = this.commonAttackKnockbackDuration,
        };

    }

    public override EnemyState GetNextAttackState()
    {
        return rangeAttackState;
    }
}
