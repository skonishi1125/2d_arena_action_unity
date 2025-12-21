using UnityEngine;

public class ObjectiveAnimationTrigger : MonoBehaviour
{
    [SerializeField] private Objective objective;

    private void Awake()
    {
        if (objective == null)
            objective = GetComponentInParent<Objective>();
    }

    public void DestroyObjectiveTrigger()
    {
        objective?.DestroySelf();
    }

}
