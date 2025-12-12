using UnityEngine;

public class EntityProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private EntityVFX entityVfx;

    // Criticalになったとき、何倍にするか 
    [SerializeField] private float criticalRate = 1.5f;

    // 敵などの弾丸,特にスキルなどではない攻撃の倍率

    // スキルを用いた弾の威力設定用変数
    // PlayerのMagicBoltなどはスキルSOに威力があるため、
    // それを保持してダメージ, KB, KB Durationに適用させる。
    // ( EntityCombat側と同じ設計:
    //  * stateからスキル情報を呼び出し、威力とKBを設定
    //  * stateを抜け出すとき、Exit()時にdefaultの設定値に戻す)
    [Header("Projectile Damage")]
    [SerializeField] private float defaultDamageMultiplier = 1f;
    [SerializeField] private Vector2 defaultKnockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private float defaultKnockbackDuration = 0.2f;
    private float currentDamageMultiplier = 1f; // 現在の攻撃のダメージ倍率
    private Vector2 currentKnockbackPower; // 現在の攻撃のダメージ倍率
    private float currentKnockbackDuration;
    private bool useCustomKnockback = false;

    // 弾の管理情報
    [Header("Projectile Setting")]
    private Entity owner; // 誰が撃ったか
    private float speed;
    private float shootDir; // 1 or -1 撃った時点での敵の向き
    private float timer;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private LayerMask whatIsTarget; // 弾の敵対象

    public bool HasCustomKnockback => useCustomKnockback;
    public Vector2 CurrentKnockbackPower => currentKnockbackPower;
    public float CurrentKnockbackDuration => currentKnockbackDuration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Prefabでも動く(生成時)
        if (!LogHelper.AssertNotNull(rb, nameof(rb), this))
            return;

        anim = GetComponentInChildren<Animator>();
        if (!LogHelper.AssertNotNull(anim, nameof(anim), this))
            return;

        // アニメ設定(必要っぽい）
        anim.SetBool("idle", true);

        currentDamageMultiplier = defaultDamageMultiplier;
        currentKnockbackPower = defaultKnockbackPower;
        currentKnockbackDuration = defaultKnockbackDuration;

    }

    // 攻撃ごとに State から呼んでもらい、現攻撃のダメージ倍率を決定
    public void SetDamageMultiplier(float multiplier)
    {
        currentDamageMultiplier = multiplier;
    }

    // MagicBoltState を抜けるときなどに元に戻す
    public void ResetDamageMultiplier()
    {
        currentDamageMultiplier = defaultDamageMultiplier;
    }

    // 攻撃ごとに State から呼ぶ
    public void SetKnockback(Vector2 power, float duration)
    {
        currentKnockbackPower = power;
        currentKnockbackDuration = duration;
        useCustomKnockback = true;
    }

    public void ResetKnockback()
    {
        currentKnockbackPower = defaultKnockbackPower;
        currentKnockbackDuration = defaultKnockbackDuration;
        useCustomKnockback = false;
    }

    public void Fire(float shootDir, float speed, Entity owner)
    {
        this.shootDir = shootDir;
        this.speed = speed;
        this.owner = owner;
        timer = lifeTime;
    }

    private void Update()
    {
        // entity.facingDirを使うと、
        // entityが反転したとき、弾も同時に反転するので射撃時の向きを使う
        rb.linearVelocity = Vector2.right * speed * shootDir;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }

    // 弾のダメージ処理
    // EntityCombatは、近接攻撃のダメージ処理担当
    private void OnTriggerEnter2D(Collider2D target)
    {
        // 弾のwhatIsTargetに指定した物以外とぶつかったときは無視
        // LayerMaskはビットフラグなので、加工した比較が必要になる
        if (((1 << target.gameObject.layer) & whatIsTarget) == 0)
        {
            Debug.Log("弾: target対象外。");
            return;
        }

        IDamagable damagable = target.GetComponent<IDamagable>();
        if (damagable == null)
            return;

        var ownerStatus = owner.GetComponent<EntityStatus>();
        var targetStatus = target.GetComponent<EntityStatus>();

        // 1. 回避判定
        if (EntityCombat.IsEvaded(ownerStatus, targetStatus))
        {
            // 相手に付与されたmissVfxを生成する
            entityVfx = target.GetComponent<EntityVFX>();
            entityVfx.CreateOnMissHitVfx(target.transform);
            Destroy(gameObject);
            return;
        }

        if (ownerStatus == null)
        {
            // 現状未使用だが、
            // 万が一ステータスが無い弾（トラップなど）の場合は
            // 固定ダメージ等にフォールバック
            var defaultCtx = new DamageContext
            {
                attacker = transform,
                damage = 5f,
                hasCustomKnockback = HasCustomKnockback,
                knockbackPower = CurrentKnockbackPower,
                knockbackDuration = CurrentKnockbackDuration,
                source = this
            };

            damagable.TakeDamage(defaultCtx);
            Destroy(gameObject);
            return;
        }

        // 2. ダメージ計算（共通ロジック）
        bool isCritical;
        float damage = EntityCombat.CalculateDamage(
            ownerStatus,
            targetStatus,
            currentDamageMultiplier,
            criticalRate,
            out isCritical
        );

        var ctx = new DamageContext
        {
            attacker = transform,
            damage = damage,
            hasCustomKnockback = HasCustomKnockback,
            knockbackPower = CurrentKnockbackPower,
            knockbackDuration = CurrentKnockbackDuration,
            source = this
        };

        //damagable?.TakeDamage(damage, owner.transform);
        damagable.TakeDamage(ctx);

        var hitVfx = owner.GetComponent<EntityVFX>();
        if (hitVfx != null)
        {
            if (isCritical)
                hitVfx.CreateOnProjectileHitVfx(target.transform);
            else
                hitVfx.CreateOnProjectileCritHitVfx(target.transform);
        }

        Destroy(gameObject);

    }


}
