using UnityEngine;

public class EntityAnimationTrigger : MonoBehaviour
{
    private Entity entity;
    protected EntityCombat entityCombat;
    [SerializeField] private EntityProjectileSpawner projectileSpawner;


    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<EntityCombat>();
    }

    protected virtual void CurrentStateTrigger()
    {
        entity.CallAnimationTrigger();
    }

    // 単発攻撃
    protected virtual void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }

    // 持続攻撃: 開始
    protected virtual void StartContinuousAttackTrigger()
    {
        entityCombat.StartContinuousAttack();
    }

    // 持続攻撃: 終了（アニメイベント用）
    protected virtual void EndContinuousAttackTrigger()
    {
        entityCombat.StopContinuousAttack();
    }

    protected virtual void ShootProjectileTrigger()
    {
        if (projectileSpawner == null)
            return;

        // State側で設定した弾の威力の取得
        // ※Player だけ特別扱いしたくないなら Entity 側に TryConsume を置く
        var player = entity as Player;
        if (player != null && player.TryConsumePendingProjectileCtx(out var ctx))
        {
            projectileSpawner.Spawn(entity, ctx);
            return;
        }

    }

}
