using System;
using UnityEngine;

public class EntityCombat : MonoBehaviour
{
    private EntityStatus entityStatus;
    private EntityVFX entityVfx;

    // 攻撃モーション時のトリガー検知に関する情報
    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;

    // クリティカル倍率
    [SerializeField] private float criticalRate = 1.5f;

    private void Awake()
    {
        entityVfx = GetComponent<EntityVFX>();
        entityStatus = GetComponent<EntityStatus>();
    }

    public void PerformAttack()
    {
        foreach (Collider2D target in GetDetectedColliders())
        {
            IDamagable damagable = target.GetComponent<IDamagable>();

            // colliderの配列からIDamagebleが見つからなければ、次のforeach対象に移る
            if (damagable == null)
                continue;

            EntityStatus targetStatus = target.GetComponent<EntityStatus>();
            float damage = CalculateDamage(entityStatus, targetStatus);

            damagable?.TakeDamage(damage, transform);
            entityVfx.CreateOnHitVfx(target.transform);
        }
    }

    private float CalculateDamage(EntityStatus attacker, EntityStatus defender)
    {
        float attack = attacker.GetAttack();
        float defense = defender.GetDefense();

        float raw = attack - defense;
        if (raw < 1f)
            raw = 1f;

        // クリティカル判定（critical を 0〜1 の確率で扱う場合）
        if (attacker != null)
        {
            float critChance = attacker.GetCritical(); // 0.0〜1.0想定
            if (UnityEngine.Random.value < critChance)
            {
                raw = raw * criticalRate; // クリティカル倍率
                Debug.Log("Critical!");
            }
        }

        Debug.Log("Damage: " + raw + " attack: " + attack + " defense: " + defense);

        return raw;
    }

    // 攻撃判定内にいたcollider全てを配列で返す。
    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }

}
