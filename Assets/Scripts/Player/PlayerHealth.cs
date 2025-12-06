using System;
using UnityEngine;

public class PlayerHealth : EntityHealth
{

    public event Action OnHealthUpdate;// 体力変化時 HealthbarUI 更新が主。
    public event Action OnDied; // 死亡時 画面シェイク,PlayerのState変更,GameOverUI他。

    protected override void ReduceHp(float damage)
    {
        //base.ReduceHp(damage);

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
    // Health -> Player.Death() -> ...
    protected override void Die()
    {
        if (isDead)
            return;

        base.Die();
        OnDied?.Invoke();
        // 例えばGameOver時、
        // GameManager.Instance.GameOver(); とも書いて呼ぶことができる。
        // ただしこれはHealthという下層が上層を呼ぶ形になっているので、
        // GameManagerから購読し、OnDiedのイベント処理として進めたほうが綺麗。
    }

    // 体力全回復(LevelUp時など)
    public void FullHeal()
    {
        currentHp = entityStatus.GetMaxHp();
        OnHealthUpdate?.Invoke(); // 体力が全回復したことの通知
    }
}
