using UnityEngine;

public class UIMiniHealthBar : MonoBehaviour
{
    private Entity entity;


    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void OnEnable()
    {
        entity.OnFlipped += PreventUIFlip;
    }

    private void OnDisable()
    {
        entity.OnFlipped -= PreventUIFlip;
    }

    private void PreventUIFlip()
    {
        // UIバーが反転して、体力の減りも反転してしまうのを防ぐ
        transform.rotation = Quaternion.identity;
    }

}
