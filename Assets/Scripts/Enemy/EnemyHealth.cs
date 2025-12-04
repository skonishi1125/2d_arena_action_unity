using UnityEngine;

public class EnemyHealth : EntityHealth
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(float damage, Transform attacker)
    {
        base.TakeDamage(damage, attacker);

        if (isDead)
            return;

        if (attacker.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(attacker);


    }
}
