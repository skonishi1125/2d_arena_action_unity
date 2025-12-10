using UnityEngine;

[System.Serializable]
public class SkillRuntimeState
{
    [Tooltip("このスキルの定義(SO)")]
    public SkillDefinition definition;

    [HideInInspector]
    public int currentLevel = 0;          // 0 = 未解放

    [HideInInspector]
    public float cooldownRemaining = 0f;  // 0以下なら即使用可能

    public SkillId Id => definition != null ? definition.id : SkillId.None;

    public bool IsUnlocked => currentLevel > 0;

    public int MaxLevel => definition != null ? definition.MaxLevel : 0;

    public SkillLevelData CurrentLevelData
        => definition != null ? definition.GetLevelData(currentLevel) : null;
}
