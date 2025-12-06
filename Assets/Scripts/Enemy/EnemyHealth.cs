using UnityEngine;

public class EnemyHealth : EntityHealth
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
        UpdateEnemyHealthBar();

    }

    protected override void ReduceHp(float damage)
    {
        base.ReduceHp(damage);
        UpdateEnemyHealthBar();
    }

    public override void TakeDamage(float damage, Transform attacker)
    {
        base.TakeDamage(damage, attacker);

        // ダメージ計算後に殴られたケースの考慮
        if (isDead)
            return;

        if (attacker.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(attacker);

    }

    private void UpdateEnemyHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHp / entityStatus.GetMaxHp();
    }

}
