using UnityEngine;

public class EntityAnimationTrigger : MonoBehaviour
{
    private Entity entity;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    protected virtual void CurrentStateTrigger()
    {
        entity.CallAnimationTrigger();
    }
}
