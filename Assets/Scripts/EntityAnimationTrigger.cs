using UnityEngine;

public class EntityAnimationTrigger : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void CurrentStateTrigger()
    {
        entity.CallAnimationTrigger();
    }
}
