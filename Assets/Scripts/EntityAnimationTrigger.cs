using UnityEngine;

public class EntityAnimationTrigger : MonoBehaviour
{
    private Entity entity;
    private EntityCombat entityCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<EntityCombat>();
    }

    protected virtual void CurrentStateTrigger()
    {
        entity.CallAnimationTrigger();
    }

    protected virtual void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }
}
