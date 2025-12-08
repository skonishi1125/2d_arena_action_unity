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

    // 突進など持続攻撃
    // ※現状darkKnightしか使ってないので、そっちに持たせるべきかも
    [Header("Continuous Attack")]
    [SerializeField] private float continuousInterval = .3f; // 何秒ごとにダメージを与えるか
    private bool isContinuousAttacking = false;
    private float continuousTimer = 0f;

    [Header("Attack Options")]
    [SerializeField] private float defaultDamageMultiplier = 1f;
    private float currentDamageMultiplier = 1f; // 現在の攻撃のダメージ倍率

    // Criticalになったとき、何倍にするか
    [SerializeField] private float criticalRate = 1.5f;

    private void Awake()
    {
        entityVfx = GetComponent<EntityVFX>();
        entityStatus = GetComponent<EntityStatus>();
        currentDamageMultiplier = defaultDamageMultiplier;
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
            float damage = CalculateDamage(entityStatus, targetStatus, out isCritical);

            damagable?.TakeDamage(damage, transform);

            if (isCritical)
                entityVfx.CreateOnCritHitVfx(target.transform);
            else
                entityVfx.CreateOnHitVfx(target.transform);

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
    private bool IsEvaded(EntityStatus attacker, EntityStatus defender)
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

    private float CalculateDamage(EntityStatus attacker, EntityStatus defender, out bool isCritical)
    {
        isCritical = false;

        float attack = attacker.GetAttack();
        float defense = defender != null ? defender.GetDefense() : 0f; // chestなど、statusを持たない場合は0

        float raw = attack - defense;
        if (raw < 1f)
            raw = 1f;

        // 攻撃個別に設定されている、倍率の反映
        raw *= currentDamageMultiplier;

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
