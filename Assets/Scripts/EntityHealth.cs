using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    private Entity entity;
    private EntityVFX entityVfx;

    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected float currentHp;
    [SerializeField] protected bool isDead;

    [Header("Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(10f, 10f);
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private float heavyKnockbackDuration = .7f;
    //高いダメージを与えた時、強くKBさせる その割合
    [SerializeField] private float heavyDamageTreshold = .3f;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<EntityVFX>();

        currentHp = maxHp;
    }

    public virtual void TakeDamage(float damage, Transform attacker)
    {
        if (isDead)
            return;

        // ノックバック
        entity?.ReceiveKnockback(
            CalculateKnockback(attacker, damage), // KB のvector
            CalculateKnockbackDuration(damage) // KB の持続時間
        );

        // 白くなる演出
        entityVfx?.PlayOnDamageVfx();

        // ダメージ計算
        ReduceHp(damage);
    }

    protected void ReduceHp(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
            Die();
    }

    protected void Die()
    {
        isDead = true;
        entity.Death();
    }

    private Vector2 CalculateKnockback(Transform attacker, float damage)
    {
        // ダメージを与えた相手が、自分より右にいるなら1
        // 左にいるなら(マイナス), -1を返すようにする
        // 左から殴られたら + のx座標, 右から殴られたら - のx座標にKB
        int direction = transform.position.x > attacker.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyKnockback(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x = knockback.x * direction;

        return knockback;
    }

    private float CalculateKnockbackDuration(float damage)
    {
        return IsHeavyKnockback(damage) ? heavyKnockbackDuration : knockbackDuration;
    }

    // ダメージの割合と最大HPを比較し、大ダメージならノックバックを高める
    private bool IsHeavyKnockback(float damage)
    {
        return damage / maxHp > heavyDamageTreshold;
    }


}
