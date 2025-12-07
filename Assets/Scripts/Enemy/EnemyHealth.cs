using System;
using UnityEngine;

public class EnemyHealth : EntityHealth
{
    private Enemy enemy;

    // 被弾時、UIMiniHealthBarを出すなど
    public event Action OnTakeDamaged;

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

        OnTakeDamaged?.Invoke();
    }

    private void UpdateEnemyHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHp / entityStatus.GetMaxHp();
    }

}
