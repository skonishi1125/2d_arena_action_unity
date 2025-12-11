using UnityEngine;

public class EntityProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private EntityVFX entityVfx;


    private float speed;
    private float shootDir; // 1 or -1 撃った時点での敵の向き
    private Entity owner; // 誰が撃ったか

    [SerializeField] private float lifeTime = 3f;
    private float timer;

    [Header("Projectile Damage")]
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float damageMultiplier = 1f;   // 弾ごとの倍率
    [SerializeField] private float criticalRate = 1.5f;     // 弾に対するクリティカル倍率

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Prefabでも動く(生成時)
        if (!LogHelper.AssertNotNull(rb, nameof(rb), this))
            return;

        anim = GetComponentInChildren<Animator>();
        if (!LogHelper.AssertNotNull(anim, nameof(anim), this))
            return;

        anim.SetBool("idle", true);
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
        // entityが反転したとき、弾も同時に反転してしまう
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
            damagable.TakeDamage(5f, owner.transform);
            Destroy(gameObject);
            return;
        }

        // 2. ダメージ計算（共通ロジック）
        bool isCritical;
        float damage = EntityCombat.CalculateDamage(
            ownerStatus,
            targetStatus,
            damageMultiplier,
            criticalRate,
            out isCritical
        );

        damagable?.TakeDamage(damage, owner.transform);

        var hitVfx = owner.GetComponent<EntityVFX>();
        if (hitVfx != null)
        {
            // 弾用のVFX(近接用は、斬撃vfxなので変えておこう)
            if (isCritical)
                hitVfx.CreateOnProjectileHitVfx(target.transform);
            else
                hitVfx.CreateOnProjectileCritHitVfx(target.transform);
        }

        Destroy(gameObject);

    }


}
