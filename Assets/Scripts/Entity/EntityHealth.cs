using UnityEngine;
using UnityEngine.UI;

public class EntityHealth : MonoBehaviour, IDamagable
{
    private Entity entity;
    private EntityVFX entityVfx;
    private EntityStatus entityStatus;

    private Slider healthBar; // using UnityEngine.UI;が必要。体力バー

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
        entityStatus = GetComponent<EntityStatus>();
        healthBar = GetComponentInChildren<Slider>();

        currentHp = entityStatus.GetMaxHp();
        UpdateHealthBar();
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
        UpdateHealthBar();

        if (currentHp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        entity.Death();
    }

    private void UpdateHealthBar()
    {
        // TODO: Player側に割り当てていないので、エラーメッセージが出る。回避用
        if (healthBar == null)
            return;

        healthBar.value = currentHp / entityStatus.GetMaxHp();
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
        return damage / entityStatus.GetMaxHp() > heavyDamageTreshold;
    }


}
