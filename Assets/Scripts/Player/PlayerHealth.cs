using System;
using UnityEngine;

public class PlayerHealth : EntityHealth
{

    // 体力が変化したとき用 HealthbarUI 更新が主。
    public event Action OnHealthUpdate;
    public event Action OnDied;

    protected override void ReduceHp(float damage)
    {
        base.ReduceHp(damage);
        OnHealthUpdate?.Invoke(); // 体力が減ったことを通知
    }

    protected override void Die()
    {
        base.Die();

        // you died!みたいなUIを併せて出す
        OnDied?.Invoke();
        // GameManager.Instance.GameOver(); とも書くことができる。
        // ただしこれはHealthという下層が上層を呼ぶ形になっているので、
        // GameManagerから購読し、処理を進めたほうがよい。
    }

    // 体力全回復(LevelUp時など)
    public void FullHeal()
    {
        currentHp = entityStatus.GetMaxHp();
        OnHealthUpdate?.Invoke(); // 体力が全回復したことの通知
    }
}
