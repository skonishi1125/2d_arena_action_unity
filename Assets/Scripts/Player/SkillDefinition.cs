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
}

[CreateAssetMenu(menuName = "Game/Skill Definition", fileName = "SkillDefinition_")]
public class SkillDefinition : ScriptableObject
{
    [Header("Identity")]
    public SkillId id;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Levels")]
    public SkillLevelData[] levels;

    public int MaxLevel => levels != null ? levels.Length : 0;

    public SkillLevelData GetLevelData(int level)
    {
        if (levels == null || levels.Length == 0)
            return null;

        // level は 1 始まりで運用する想定
        int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
        return levels[index];
    }
}
