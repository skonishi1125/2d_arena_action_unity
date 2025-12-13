using UnityEngine;

public class DemonGunnerAnimationTrigger : EntityAnimationTrigger
{
    private DemonGunner demonGunner;

    protected override void Awake()
    {
        base.Awake();
        demonGunner = GetComponentInParent<DemonGunner>();
        if (!LogHelper.AssertNotNull(demonGunner, nameof(demonGunner), this))
            return;

    }

    protected virtual void ShootProjectileTrigger()
    {
        if (projectileSpawner == null)
            return;

        projectileSpawner.Spawn(
            demonGunner,
            demonGunner.PendingProjectileSpawnRequest
        );

    }
}
