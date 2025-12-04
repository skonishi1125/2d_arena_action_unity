using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected bool isDead;

    public virtual void TakeDamage(float damage, Transform attacker)
    {
        if (isDead)
            return;

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
