using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemDefinition definition;
    [SerializeField] private LayerMask whatIsTarget;


    public static event Action OnTakeItem;

    private void Reset()
    {
        // ColliderはTrigger推奨
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & whatIsTarget) == 0)
        {
            //Debug.Log("playerではないのでスキップ");
            return;
        }

        if (definition != null)
        {
            definition.Apply(other.gameObject);
            OnTakeItem?.Invoke();
        }

        Destroy(gameObject);
    }
}
