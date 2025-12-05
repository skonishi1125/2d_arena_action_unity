using System;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private EntityStatus entityStatus;

    [SerializeField] private PlayerLevelTable levelTable;

    public int Level { get; private set; } = 1;
    public int CurrentExp { get; private set; }

    // LvUP時のアクションイベント
    // GameManagerでLvUP時のUIを出すとか。
    public event Action<int> OnLevelUp;

    private void Awake()
    {
        entityStatus = GetComponent<EntityStatus>();
    }

    private void Start()
    {
        ApplyLevelStatus();
    }

    private void ApplyLevelStatus()
    {
        var levelInfo = levelTable.GetLevelOfInfo(Level);

        // Status 側の基礎値をアップデート
        entityStatus.maxHp.SetBaseValue(levelInfo.maxHp);
        entityStatus.regenHp.SetBaseValue(levelInfo.regenHp);
        entityStatus.maxMp.SetBaseValue(levelInfo.maxMp);
        entityStatus.regenMp.SetBaseValue(levelInfo.regenMp);

        entityStatus.attack.SetBaseValue(levelInfo.attack);
        entityStatus.defense.SetBaseValue(levelInfo.defense);
        entityStatus.evasion.SetBaseValue(levelInfo.evasion);
        entityStatus.critical.SetBaseValue(levelInfo.critical);

    }

    public void AddExp(int amount)
    {
        if (amount <= 0)
            return;

        CurrentExp += amount;
        // 必要経験値を満たす限りレベルアップ
        while (true)
        {
            var entry = levelTable.GetLevelOfInfo(Level);

            // 最終レベルならそれ以上は上げない
            if (Level >= levelTable.MaxLevel)
                break;

            // レベルアップ
            if (CurrentExp >= entry.requiredExp)
            {
                CurrentExp -= entry.requiredExp;
                Level++;

                ApplyLevelStatus();
                OnLevelUp?.Invoke(Level);
            }
            else
            {
                break;
            }
        }
    }



}
