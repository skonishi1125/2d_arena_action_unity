using System;
using UnityEngine;

public class ObjectiveHealth : MonoBehaviour, IDamagable
{
    private Objective objective;

    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float currentHp;
    [SerializeField] private bool isDestroyed;

    // HPバーの更新とかに使う
    public event Action<float, float> OnHpChanged; // current, max

    // ゲームオーバー通知とかに使う
    public event Action<ObjectiveHealth> OnDestroyed;

    public bool IsDestroyed => isDestroyed;
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    private void Awake()
    {
        objective = GetComponent<Objective>();
        if (!LogHelper.AssertNotNull(objective, nameof(objective), this))
            return;

        currentHp = maxHp;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(DamageContext ctx)
    {
        if (isDestroyed)
            return;

        currentHp -= ctx.damage;
        objective.entityVfx.PlayOnDamageVfx();

        OnHpChanged?.Invoke(currentHp, maxHp);

        if (currentHp <= 0)
        {
            objective.anim.SetBool("objectiveDestroy", true);
            isDestroyed = true;
            OnDestroyed?.Invoke(this);
        }
    }
}
