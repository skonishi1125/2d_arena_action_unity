using UnityEngine;

[System.Serializable]
public class SkillLevelData
{
    [Min(1)]
    public int level = 1;

    [Header("Combat")]
    public float damageMultiplier = 1f;
    public Vector2 knockbackPower = Vector2.zero;
    public float knockbackDuration = 0.15f;

    [Header("Cost / Cooldown")]
    public float cooldownTime = 1f;
    public float manaCost = 0f;

    [Header("Description")]
    [TextArea]
    public string levelDescription;   // 「Lv1ならLv1の説明を書く
}

// どのスロットのスキルに該当するのか
public enum SkillSlot {
    None,
    Z,
    D,
    V
}

[CreateAssetMenu(menuName = "Game/Skill Definition", fileName = "SkillDefinition_")]
public class SkillDefinition : ScriptableObject
{
    [Header("Identity")]
    public SkillId id;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public SkillSlot slot = SkillSlot.None;
    public bool exclusiveInSlot = true;
    public EntityProjectile projectilePrefab; // 弾スキルなら持たせる。

    [Header("Levels")]
    public SkillLevelData[] levels;

    public int MaxLevel => levels != null ? levels.Length : 0;

    public SkillLevelData GetLevelData(int level)
    {
        if (levels == null || levels.Length == 0)
            return null;

        // 1〜MaxLevel の範囲だけ有効
        int index = level - 1;
        if (index < 0 || index >= levels.Length)
            return null;

        return levels[index];
    }
}
