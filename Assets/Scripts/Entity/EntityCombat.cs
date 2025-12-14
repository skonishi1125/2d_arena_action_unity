using UnityEngine;

public class EntityCombat : MonoBehaviour
{
    public enum HitShape { Circle, Box }

    private EntityStatus entityStatus;
    private EntityVFX entityVfx;

    // Criticalになったとき、何倍にするか
    [SerializeField] private float criticalRate = 1.5f;

    // 攻撃モーション時のトリガー検知に関する情報
    [Header("Target detection")]
    [SerializeField] private HitShape defaultHitShape = HitShape.Circle;
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private Vector2 targetCheckBoxSize = new(3f, 1f); // 追加
    [SerializeField] private float targetCheckBoxAngle = 0f;       // 追加（基本0でOK
    [SerializeField] private LayerMask whatIsTarget;

    private bool useCustomHitbox;
    private HitShape currentHitShape;
    private float currentRadius;
    private Vector2 currentBoxSize;
    private float currentBoxAngle;

    // 現在の中心
    private Transform currentTargetCheck;
    private bool useCustomTargetCheck;

    // 突進など持続攻撃
    // ※現状darkKnightしか使ってないので、そっちに持たせるべきかも
    [Header("Continuous Attack")]
    [SerializeField] private float continuousInterval = .3f; // 何秒ごとにダメージを与えるか
    private bool isContinuousAttacking = false;
    private float continuousTimer = 0f;

    [Header("Attack Options")]
    [SerializeField] private float defaultDamageMultiplier = 1f;
    private float currentDamageMultiplier = 1f; // 現在の攻撃のダメージ倍率

    [Header("KB Options")]
    [SerializeField] private Vector2 defaultKnockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private float defaultKnockbackDuration = 0.2f;

    private Vector2 currentKnockbackPower; // 現在の攻撃のダメージ倍率
    private float currentKnockbackDuration;
    private bool useCustomKnockback = false;

    public bool HasCustomKnockback => useCustomKnockback;
    public Vector2 CurrentKnockbackPower => currentKnockbackPower;
    public float CurrentKnockbackDuration => currentKnockbackDuration;


    private void Awake()
    {
        entityVfx = GetComponent<EntityVFX>();
        entityStatus = GetComponent<EntityStatus>();

        currentDamageMultiplier = defaultDamageMultiplier;
        currentKnockbackPower = defaultKnockbackPower;
        currentKnockbackDuration = defaultKnockbackDuration;
    }

    private void Update()
    {
        HandleContinuousAttack();
    }

    // 攻撃ごとに State から呼んでもらい、現攻撃のダメージ倍率を決定
    public void SetDamageMultiplier(float multiplier)
    {
        currentDamageMultiplier = multiplier;
    }

    // AttackState を抜けるときなどに元に戻す
    public void ResetDamageMultiplier()
    {
        currentDamageMultiplier = defaultDamageMultiplier;
    }

    // ▼ 攻撃ごとに State から呼ぶ
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

    // 通常 単発攻撃
    public void PerformAttack()
    {
        foreach (Collider2D target in GetDetectedColliders())
        {
            IDamagable damagable = target.GetComponent<IDamagable>();

            // colliderの配列からIDamagebleが見つからなければ、次のforeach対象に移る
            if (damagable == null)
                continue;

            EntityStatus targetStatus = target.GetComponent<EntityStatus>();

            // 1. 回避判定
            if (IsEvaded(entityStatus, targetStatus))
            {
                entityVfx.CreateOnMissHitVfx(target.transform);
                continue;
            }

            // 2. ダメージ計算
            bool isCritical = false;
            float damage = CalculateDamage(
                entityStatus,
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

            damagable.TakeDamage(ctx);


            if (isCritical)
            {
                entityVfx.CreateOnCritHitVfx(target.transform);
                entityVfx.CreateOnCritDamageNumberVfx(target.transform, damage);
            }
            else
            {
                entityVfx.CreateOnHitVfx(target.transform);
                entityVfx.CreateOnDamageNumberVfx(target.transform, damage);
            }

        }
    }

    // 持続攻撃中かどうかの判定
    private void HandleContinuousAttack()
    {
        if (!isContinuousAttacking)
            return;

        continuousTimer -= Time.deltaTime;
        if (continuousTimer <= 0f)
        {
            continuousTimer = continuousInterval;
            PerformAttack();
        }
    }

    // 持続攻撃開始
    public void StartContinuousAttack()
    {
        continuousTimer = 0f;// すぐ初回判定を出す
        isContinuousAttacking = true;
    }

    // 持続攻撃終了
    public void StopContinuousAttack()
    {
        isContinuousAttacking = false;
    }


    // 回避率を計算し、その結果を返す
    public static bool IsEvaded(EntityStatus attacker, EntityStatus defender)
    {
        if (defender == null)
            return false;

        // 0.0 から 1.0  chestなど、statusを持たない場合は0f
        float evasion = defender != null ? defender.GetEvasion() : 0f;
        if (evasion <= 0f)
            return false;

        return Random.value < evasion;
    }

    // 実ダメージを返す

    public static float CalculateDamage(
        EntityStatus attacker,
        EntityStatus defender,
        float damageMultiplier,
        float criticalRate,
        out bool isCritical
    )
    {
        isCritical = false;

        float attack = attacker.GetAttack();
        float defense = defender != null ? defender.GetDefense() : 0f; // chestなど、statusを持たない場合は0

        float raw = attack - defense;
        if (raw < 1f)
            raw = 1f;

        // 攻撃個別に設定されている、倍率の反映
        raw *= damageMultiplier;

        // クリティカル判定（critical を 0〜1 の確率で扱う場合）
        if (attacker != null)
        {
            float critChance = attacker.GetCritical(); // 0.0〜1.0想定
            if (Random.value < critChance)
            {
                raw = raw * criticalRate; // クリティカル倍率
                isCritical = true;
            }
        }

        Debug.Log("Damage: " + raw + " attack: " + attack + " defense: " + defense + " isCritical: " + isCritical);

        return raw;
    }

    // 追加：中心Transformを指定できる
    public void SetHitboxCircle(Transform centerCheck, float radius)
    {
        useCustomHitbox = true;
        currentHitShape = HitShape.Circle;
        currentRadius = radius;

        useCustomTargetCheck = centerCheck != null;
        currentTargetCheck = centerCheck;
    }

    public void SetHitboxBox(Transform centerCheck, Vector2 size, float angle = 0f)
    {
        useCustomHitbox = true;
        currentHitShape = HitShape.Box;
        currentBoxSize = size;
        currentBoxAngle = angle;

        useCustomTargetCheck = centerCheck != null;
        currentTargetCheck = centerCheck;
    }

    public void ResetHitbox()
    {
        useCustomHitbox = false;
        useCustomTargetCheck = false;
        currentTargetCheck = null;
    }

    private Collider2D[] GetDetectedColliders()
    {
        var shape = useCustomHitbox ? currentHitShape : defaultHitShape;
        var center = GetHitCenter();

        switch (shape)
        {
            case HitShape.Circle:
                float r = useCustomHitbox ? currentRadius : targetCheckRadius;
                return Physics2D.OverlapCircleAll(center, r, whatIsTarget);

            case HitShape.Box:
                Vector2 size = useCustomHitbox ? currentBoxSize : targetCheckBoxSize;
                float angle = useCustomHitbox ? currentBoxAngle : targetCheckBoxAngle;
                return Physics2D.OverlapBoxAll(center, size, angle, whatIsTarget);

            default:
                return System.Array.Empty<Collider2D>();
        }
    }

    private Vector2 GetHitCenter()
    {
        var t = (useCustomTargetCheck && currentTargetCheck != null) ? currentTargetCheck : targetCheck;
        return t != null ? (Vector2)t.position : (Vector2)transform.position;
    }

    private Transform GetCenterTransform()
    {
        if (useCustomTargetCheck && currentTargetCheck != null)
            return currentTargetCheck;

        return targetCheck != null ? targetCheck : transform;
    }

    private Vector3 GetCenterPosition()
    {
        return GetCenterTransform().position;
    }
    private void OnDrawGizmos()
    {
        // 再生中は現在の設定、停止中はデフォルト設定で描く
        bool playing = Application.isPlaying;

        var shape = playing
            ? (useCustomHitbox ? currentHitShape : defaultHitShape)
            : defaultHitShape;

        var center = playing ? GetCenterPosition()
                             : (targetCheck != null ? targetCheck.position : transform.position);

        Gizmos.color = Color.red;

        if (shape == HitShape.Circle)
        {
            float r = playing
                ? (useCustomHitbox ? currentRadius : targetCheckRadius)
                : targetCheckRadius;

            Gizmos.DrawWireSphere(center, r);
        }
        else
        {
            Vector2 size2 = playing
                ? (useCustomHitbox ? currentBoxSize : targetCheckBoxSize)
                : targetCheckBoxSize;

            float angle = playing
                ? (useCustomHitbox ? currentBoxAngle : targetCheckBoxAngle)
                : targetCheckBoxAngle;

            var old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size2.x, size2.y, 0f));
            Gizmos.matrix = old;
        }
    }


}
