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
        if (attacker.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(attacker);

        base.TakeDamage(damage, attacker);

    }
}
