using System;
using UnityEngine;

public class PlayerHealth : EntityHealth
{

    // 体力UI 更新用
    public event Action OnHealthUpdate;

    protected override void ReduceHp(float damage)
    {
        base.ReduceHp(damage);
        OnHealthUpdate?.Invoke();
    }

    protected override void Die()
    {
        base.Die();

        // you died!みたいなUIを併せて出す
        GameManager.Instance.GameOver();
    }
}
