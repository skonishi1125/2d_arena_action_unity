using System;
using UnityEngine;

public class ObjectiveHealth : MonoBehaviour, IDamagable
{
    private Objective objective;

    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float currentHp;
    [SerializeField] private bool isDestroyed;

    [SerializeField] private Collider2D damageCollider; // ←殴られる用だけ

    // HPバーの更新とかに使う
    public event Action OnHealthUpdate;

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
        OnHealthUpdate?.Invoke();
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }

    public void TakeDamage(DamageContext ctx)
    {
        if (isDestroyed)
            return;

        currentHp -= ctx.damage;
        objective.entityVfx.PlayOnDamageVfx();


        if (currentHp <= 0)
        {
            currentHp = 0;
            objective.anim.SetBool("objectiveDestroy", true);
            isDestroyed = true;

            if (damageCollider != null)
                damageCollider.enabled = false;

            OnDestroyed?.Invoke(this);
        }

        OnHealthUpdate?.Invoke();

    }

}
