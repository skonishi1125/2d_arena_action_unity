using UnityEngine;

public interface IDamagable
{
    //public void TakeDamage(float damage, Transform attacker);
    public void TakeDamage(DamageContext ctx);
}
