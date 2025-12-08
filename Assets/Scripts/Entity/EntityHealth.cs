using System;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealth : MonoBehaviour, IDamagable
{
    private Entity entity;
    private EntityVFX entityVfx;
    protected EntityStatus entityStatus;

    protected Slider healthBar; // using UnityEngine.UI;が必要。体力バー

    [SerializeField] protected float currentHp;
    [SerializeField] protected bool isDead;

    // 現在EntityCombat側で、攻撃側がKBを決定できる
    // 攻撃側で指定されていないとき、こちらのKB設定値が考慮される。
    [Header("Old Damage Knockback")]
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
    }

    public virtual void TakeDamage(float damage, Transform attacker)
    {
        if (isDead)
            return;

        // ノックバック
        entity?.ReceiveKnockback(
            CalculateKnockback(attacker, damage),
            CalculateKnockbackDuration(attacker, damage)
        );


        // 白くなる演出
        entityVfx?.PlayOnDamageVfx();

        // ダメージ計算
        ReduceHp(damage);
    }


    protected virtual void ReduceHp(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        entity.Death();
    }

    // 画面UIの体力バーから参照する
    public float GetCurrentHp()
    {
        return currentHp;
    }

    private Vector2 CalculateKnockback(Transform attacker, float damage)
    {
        int direction = transform.position.x > attacker.position.x ? 1 : -1;

        // 攻撃側の EntityCombat を見る
        var attackerCombat = attacker.GetComponent<EntityCombat>();
        if (attackerCombat != null && attackerCombat.HasCustomKnockback)
        {
            Vector2 kb = attackerCombat.CurrentKnockbackPower;
            kb.x *= direction;
            return kb;
        }

        // それ以外は従来の heavy / normal ロジック
        Vector2 basePower = IsHeavyKnockback(damage) ? heavyKnockbackPower : knockbackPower;
        basePower.x *= direction;
        return basePower;
    }

    private float CalculateKnockbackDuration(Transform attacker, float damage)
    {
        // 攻撃側
        var attackerCombat = attacker.GetComponent<EntityCombat>();
        if (attackerCombat != null && attackerCombat.HasCustomKnockback)
        {
            return attackerCombat.CurrentKnockbackDuration;
        }

        // それ以外は従来の heavy / normal ロジック
        return IsHeavyKnockback(damage) ? heavyKnockbackDuration : knockbackDuration;
    }


    // 旧ロジック
    // ダメージの割合と最大HPを比較し、大ダメージならノックバックを高める
    private bool IsHeavyKnockback(float damage)
    {
        return damage / entityStatus.GetMaxHp() > heavyDamageTreshold;
    }


}
