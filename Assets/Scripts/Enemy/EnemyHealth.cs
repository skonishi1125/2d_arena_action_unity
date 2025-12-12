using System;
using UnityEngine;

public class EnemyHealth : EntityHealth
{
    private Enemy enemy;

    // Enemy.csに持たせたIsBossを扱う
    public bool IsBoss;

    // 被弾時、UIMiniHealthBarを出すなどの用途
    // 特定のEnemyHealthだけに対するイベントなので、static不要
    public event Action OnTakeDamaged;

    // WaveManager側で討伐したことの通知などの用途
    // どのEnemyHealthにも関わらず紐づけたいので、staticを付与する。
    public static event Action<EnemyHealth> OnAnyEnemyDied;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponent<Enemy>();
        if (!LogHelper.AssertNotNull(enemy, nameof(enemy), this))
            return;

        IsBoss = enemy.IsBoss;
        UpdateEnemyHealthBar();

    }

    protected override void ReduceHp(float damage)
    {
        base.ReduceHp(damage);
        UpdateEnemyHealthBar();
    }

    //public override void TakeDamage(float damage, Transform attacker)
    //{
    //    base.TakeDamage(damage, attacker);

    //    // ダメージ計算後に殴られたケースの考慮
    //    if (isDead)
    //        return;

    //    if (attacker.GetComponent<Player>() != null)
    //        enemy.TryEnterBattleState(attacker);

    //    OnTakeDamaged?.Invoke();
    //}

    public override void TakeDamage(DamageContext ctx)
    {
        base.TakeDamage(ctx);

        // ダメージ計算後に殴られたケースの考慮
        if (isDead)
            return;

        var attacker = ctx.attacker;
        Debug.Log(attacker);

        if (attacker.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(attacker);

        OnTakeDamaged?.Invoke();
    }


    protected override void Die()
    {
        base.Die();
        OnAnyEnemyDied?.Invoke(this);
    }

    private void UpdateEnemyHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHp / entityStatus.GetMaxHp();
    }

}
