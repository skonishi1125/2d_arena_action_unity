using UnityEngine;

public class DemonGunner : Enemy
{
    public ProjectileSpawnRequest PendingProjectileSpawnRequest { get; private set; }

    // 雑魚敵なので、自身に持たせてしまおう
    [SerializeField] private EntityProjectile projectilePrefab;

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
        PendingProjectileSpawnRequest = new ProjectileSpawnRequest
        {
            prefab = projectilePrefab,
            damage =
            {
                damageMultiplier = commonAttackDamageMultiplier,
                hasCustomKnockback = true,
                knockbackPower = commonAttackKnockbackPower,
                knockbackDuration = commonAttackKnockbackDuration,
            },
            speedOverride = 5f,
            pierceGround = false,
            pierceTargets = false,
        };

    }

    public override EnemyState GetNextAttackState()
    {
        return rangeAttackState;
    }
}
