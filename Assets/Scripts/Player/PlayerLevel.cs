using System;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private EntityStatus entityStatus;
    private PlayerHealth playerHealth;

    [SerializeField] private PlayerLevelTable levelTable;
    public int SkillPoints { get; private set; }

    public int Level { get; private set; } = 1;
    public int CurrentExp { get; private set; }

    // UIStatusMenu表示用
    public int CurrentTotalExp { get; private set; } // UI表示用

    public int CurrentRequiredExp
    {
        get
        {
            var entry = levelTable.GetLevelOfInfo(Level);
            return entry.requiredExp;
        }
    }
    public int NextLevelRequiredExp => GetNeedExp(Level + 1);


    // LvUP時のアクションイベント
    // GameManagerでLvUP時のUIを出すとかStatusMenuの更新。
    public event Action<int> OnLevelUp;

    // 経験値変化のイベント
    // currentExp, reuiredExp, StatusMenuの更新など
    public event Action<int, int> OnExpChanged;

    // UI更新
    public event Action<int> OnSkillPointsChanged;

    private void Awake()
    {
        entityStatus = GetComponent<EntityStatus>();
        playerHealth = GetComponent<PlayerHealth>();

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

                // SP獲得
                AddSkillPoint(1);

                ApplyLevelStatus();
                playerHealth.FullHeal();
                OnLevelUp?.Invoke(Level);
            }
            else
            {
                break;
            }
        }

        // 経験値が更新されたことで発生するイベントの発火
        // EXPバーの更新など。
        var currentRequired = levelTable.GetLevelOfInfo(Level).requiredExp;
        OnExpChanged?.Invoke(CurrentExp, currentRequired);

    }

    public int GetNeedExp(int level)
    {
        var clamped = Mathf.Clamp(level, 1, levelTable.MaxLevel);
        return levelTable.GetLevelOfInfo(clamped).requiredExp;
    }


    // SPの獲得
    private void AddSkillPoint(int amount)
    {
        if (amount <= 0)
            return;

        SkillPoints += amount;
        OnSkillPointsChanged?.Invoke(SkillPoints);
    }

    public bool TrySpendSkillPoints(int cost)
    {
        if (cost <= 0)
            return true;

        if (SkillPoints < cost)
            return false;

        SkillPoints -= cost;
        OnSkillPointsChanged?.Invoke(SkillPoints);
        return true;
    }

}
