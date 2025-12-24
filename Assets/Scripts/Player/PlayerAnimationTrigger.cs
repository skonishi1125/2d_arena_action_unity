using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerAnimationTrigger : EntityAnimationTrigger
{
    private Player player;
    private PlayerSFX sfx;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        if (!LogHelper.AssertNotNull(player, nameof(player), this))
            return;

        sfx = GetComponentInParent<PlayerSFX>();
        if (!LogHelper.AssertNotNull(sfx, nameof(sfx), this))
            return;

    }

    protected override void AttackTrigger()
    {
        sfx?.PlayAttack();
        base.AttackTrigger();
    }

    private void DashStartAttackTrigger()
    {
        if (player.Skill.DashHasStartAttack())
            entityCombat.PerformAttack();
    }

    private void DashEndAttackTrigger()
    {
        if (player.Skill.DashHasEndAttack())
            entityCombat.PerformAttack();
    }

    protected virtual void ShootProjectileTrigger()
    {
        if (projectileSpawner == null)
            return;

        // State側で設定した弾の威力の取得
        if (player != null && player.TryConsumePendingProjectileRequest(out var ctx))
        {
            projectileSpawner.Spawn(player, ctx);
            return;
        }
        sfx?.PlayMagic();


    }

}

