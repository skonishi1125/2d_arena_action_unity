using UnityEngine;

public class EntityAnimationTrigger : MonoBehaviour
{
    private Entity entity;
    protected EntityCombat entityCombat;
    [SerializeField] protected EntityProjectileSpawner projectileSpawner;


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



}
