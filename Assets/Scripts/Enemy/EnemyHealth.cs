using UnityEngine;

public class EnemyHealth : EntityHealth
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(float damage, Transform attacker)
    {

        if (attacker.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(attacker);

        base.TakeDamage(damage, attacker);



    }
}
