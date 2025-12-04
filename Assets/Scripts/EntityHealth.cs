using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    private EntityVFX entityVfx;

    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected bool isDead;

    protected virtual void Awake()
    {
        entityVfx = GetComponent<EntityVFX>();
    }

    public virtual void TakeDamage(float damage, Transform attacker)
    {
        if (isDead)
            return;

        entityVfx?.PlayOnDamageVfx();
        ReduceHp(damage);
    }

    protected void ReduceHp(float damage)
    {
        maxHp -= damage;
        if (maxHp <= 0)
            Die();
    }

    protected void Die()
    {
        isDead = true;
        Debug.Log("die");
    }


}
