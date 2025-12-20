using UnityEngine;

public class ChestAnimationTrigger : MonoBehaviour
{
    [SerializeField] private Chest chest;

    private void Awake()
    {
        if (chest == null)
            chest = GetComponentInParent<Chest>();
    }

    public void AE_SpawnItem()
    {
        chest?.SpawnRandomItem();
    }

    public void AE_DestroyChest()
    {
        chest?.DestroySelf();
    }

}
