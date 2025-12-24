using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerHealth : EntityHealth
{
    [Header("HP Regen")]
    [SerializeField] private float regenInterval = 5f;   // 5秒ごと
    [SerializeField] private float regenPercent = 0.01f; // 最大HPの1%
    private float regenTimer = 0f;

    public event Action OnTakeDamage;
    public event Action OnHealthUpdate;// 体力変化時 HealthbarUI 更新が主。
    public event Action OnDied; // 死亡時 画面シェイク,PlayerのState変更,GameOverUI他。

    protected virtual void Update()
    {
        ApplyRegen(Time.deltaTime);
    }

    private void ApplyRegen(float dt)
    {
        if (isDead) return;

        float maxHp = entityStatus.GetMaxHp();
        if (currentHp >= maxHp)
        {
            regenTimer = 0f;
            return;
        }

        regenTimer += dt;
        if (regenTimer < regenInterval) return;

        // dtが大きくても取りこぼさない
        bool healed = false;
        while (regenTimer >= regenInterval && currentHp < maxHp)
        {
            regenTimer -= regenInterval;

            float heal = Mathf.Max(1.0f, Mathf.Floor(maxHp * regenPercent)); // 1%が小数なら1
            currentHp = Mathf.Min(currentHp + heal, maxHp);
            healed = true;
        }

        if (healed)
            OnHealthUpdate?.Invoke();
    }


    public override void TakeDamage(DamageContext ctx)
    {
        base.TakeDamage(ctx);
        OnTakeDamage?.Invoke();
    }

    protected override void ReduceHp(float damage)
    {
        // イベント関連の設計のため、自身でダメージ計算を済ませる
        // base.ReduceHp(damage);

        if (isDead)
            return;

        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();// ここで自分の Die() を呼ぶ
        }

        OnHealthUpdate?.Invoke(); // 体力が減ったことを通知
    }

    // Player.Deathとの違い
    // 数値的な事実を扱うようにする。死亡したかどうかの決定の始点はここ。
    // PlayerHealth.Die() -> base.Die() -> Player.Death() -> ...
    protected override void Die()
    {
        if (isDead)
            return;

        base.Die();
        OnDied?.Invoke();
        // 例えばGameOver時、GameManagerの関数を実行したければ、
        // GameManager.Instance.GameOver(); とも書いて呼ぶことができる。
        // ただしこれはHealthという下層が上層を呼ぶ形になっているので、
        // GameManagerから購読し、OnDiedのイベント処理として進めたほうが綺麗。
    }

    // 回復(アイテムなど)
    public void Heal(float healMultiplier)
    {
        if (isDead)
            return;

        //currentHp = Mathf.Min(currentHp + amount, entityStatus.GetMaxHp());

        float healPoint = entityStatus.GetMaxHp() * healMultiplier;
        healPoint = Mathf.Floor(healPoint + 0.5f);

        currentHp += healPoint;
        // 四捨五入して、4.0 など、綺麗な整数(ただし、float)の形にする
        if (currentHp >= entityStatus.GetMaxHp())
            currentHp = entityStatus.GetMaxHp();
        OnHealthUpdate?.Invoke(); // 体力が回復したことの通知
    }

    // 体力全回復(LevelUp時など)
    public void FullHeal()
    {
        currentHp = entityStatus.GetMaxHp();
        OnHealthUpdate?.Invoke(); // 体力が全回復したことの通知
    }



}
