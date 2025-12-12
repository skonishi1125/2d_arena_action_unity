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

    //public virtual void TakeDamage(float damage, Transform attacker)
    //{
    //    if (isDead)
    //        return;

    //    // ノックバック
    //    entity?.ReceiveKnockback(
    //        CalculateKnockback(attacker, damage),
    //        CalculateKnockbackDuration(attacker, damage)
    //    );

    //    // 白くなる演出
    //    entityVfx?.PlayOnDamageVfx();

    //    // ダメージ計算
    //    ReduceHp(damage);
    //}

    public virtual void TakeDamage(DamageContext ctx)
    {
        if (isDead) return;

        // KBが指定されている場合、そちらを取る
        Vector2 power = ctx.hasCustomKnockback
            ? ctx.knockbackPower
            : (IsHeavyKnockback(ctx.damage) ? heavyKnockbackPower : knockbackPower);

        float duration = ctx.hasCustomKnockback
            ? ctx.knockbackDuration
            : (IsHeavyKnockback(ctx.damage) ? heavyKnockbackDuration : knockbackDuration);

        // 方向付け
        int direction = transform.position.x > ctx.attacker.position.x ? 1 : -1;

        float randomScale = UnityEngine.Random.Range(0.9f, 1.1f);
        power *= randomScale;
        power.x *= direction;

        float randomDurationScale = UnityEngine.Random.Range(0.9f, 1.1f);
        duration *= randomDurationScale;

        entity?.ReceiveKnockback(power, duration);
        entityVfx?.PlayOnDamageVfx();
        ReduceHp(ctx.damage);

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

    //private Vector2 CalculateKnockback(Transform attacker, float damage)
    //{
    //    int direction = transform.position.x > attacker.position.x ? 1 : -1;

    //    // 攻撃情報を見る
    //    // 近接攻撃 EntityCombat か、遠距離弾 EntityProjectile から発生したものか。
    //    // 現状近接攻撃のみしか考慮できていない
 
    //    // 攻撃側の EntityCombat を見る
    //    var attackerCombat = attacker.GetComponent<EntityCombat>();
    //    Vector2 basePower;

    //    if (attackerCombat != null && attackerCombat.HasCustomKnockback)
    //    {
    //        // KBを持つ技(基本こっち)
    //        basePower = attackerCombat.CurrentKnockbackPower;
    //    }
    //    else
    //    {
    //        // 設定のない技は、受け手の持つ基礎KBで決める
    //        basePower = IsHeavyKnockback(damage) ? heavyKnockbackPower : knockbackPower;
    //    }

    //    // ★ 強さに少しばらつきを与える（例：±10%）
    //    // 敵をまとめて飛ばしたときに、アニメなどの重なりを防止する
    //    // (1体に見えるのに、実際は何匹も重なっているような挙動を防ぐ）
    //    float randomScale = UnityEngine.Random.Range(0.9f, 1.1f);
    //    basePower *= randomScale;

    //    // 向きだけ最後に乗せる
    //    basePower.x *= direction;
    //    return basePower;

    //}

    //private float CalculateKnockbackDuration(Transform attacker, float damage)
    //{
    //    // 攻撃側
    //    var attackerCombat = attacker.GetComponent<EntityCombat>();
    //    float finalKnockbackDuration;
    //    if (attackerCombat != null && attackerCombat.HasCustomKnockback)
    //    {
    //        finalKnockbackDuration = attackerCombat.CurrentKnockbackDuration;
    //    } else
    //    {
    //        finalKnockbackDuration = IsHeavyKnockback(damage) ? heavyKnockbackDuration : knockbackDuration;
    //    }

    //    // ★ 強さに少しばらつきを与える（例：±10%）
    //    float randomScale = UnityEngine.Random.Range(0.9f, 1.1f);
    //    finalKnockbackDuration *= randomScale;

    //    // それ以外は従来の heavy / normal ロジック
    //    return finalKnockbackDuration;
    //}


    // 旧ロジック
    // ダメージの割合と最大HPを比較し、大ダメージならノックバックを高める
    private bool IsHeavyKnockback(float damage)
    {
        return damage / entityStatus.GetMaxHp() > heavyDamageTreshold;
    }


}
