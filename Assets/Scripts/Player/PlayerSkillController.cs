using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Skill States (Definition + Runtime)")]
    [SerializeField]
    private SkillRuntimeState[] skillStates;

    private readonly Dictionary<SkillId, SkillRuntimeState> skillMap
        = new Dictionary<SkillId, SkillRuntimeState>();

    private void Awake()
    {
        skillMap.Clear();

        foreach (var state in skillStates)
        {
            if (state == null || state.definition == null)
                continue;

            var id = state.definition.id;
            if (id == SkillId.None)
                continue;

            if (skillMap.ContainsKey(id))
            {
                Debug.LogWarning($"Duplicate SkillId in PlayerSkillController: {id}");
                continue;
            }

            skillMap.Add(id, state);
        }
    }

    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);
    }

    // ========= 基本取得まわり =========

    public SkillRuntimeState GetState(SkillId id)
    {
        if (id == SkillId.None)
            return null;

        skillMap.TryGetValue(id, out var state);
        return state;
    }

    public int GetLevel(SkillId id)
    {
        var state = GetState(id);
        return state != null ? state.currentLevel : 0;
    }

    public int GetMaxLevel(SkillId id)
    {
        var state = GetState(id);
        return state != null ? state.MaxLevel : 0;
    }

    public bool IsUnlocked(SkillId id)
        => GetLevel(id) > 0;

    // ========= レベルアップ / アンロック =========

    public bool CanLevelUp(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return false;

        return state.currentLevel < state.MaxLevel;
    }

    public bool LevelUp(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return false;

        if (!CanLevelUp(id))
            return false;

        state.currentLevel++;
        Debug.Log($"Skill {id} leveled up to {state.currentLevel}");
        return true;
    }

    // ========= 使用可否 / クールダウン =========

    public bool CanUse(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return false;

        if (state.currentLevel <= 0)
            return false;

        if (state.cooldownRemaining > 0f)
            return false;

        // 将来的に MP などの条件もここで判定
        return true;
    }

    public void OnUse(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return;

        var levelData = state.CurrentLevelData;
        if (levelData == null)
            return;

        // クールダウン開始
        state.cooldownRemaining = levelData.cooldownTime;
    }

    private void UpdateCooldowns(float deltaTime)
    {
        foreach (var state in skillStates)
        {
            if (state.cooldownRemaining > 0f)
            {
                state.cooldownRemaining -= deltaTime;
                if (state.cooldownRemaining < 0f)
                    state.cooldownRemaining = 0f;
            }
        }
    }

    /// <summary>
    /// UI 用。1 = クールダウン完了、0 = 使った直後
    /// </summary>
    public float GetCooldownRatio(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return 1f;

        var levelData = state.CurrentLevelData;
        if (levelData == null || levelData.cooldownTime <= 0f)
            return 1f;

        if (state.cooldownRemaining <= 0f)
            return 1f;

        float t = Mathf.Clamp01(1f - (state.cooldownRemaining / levelData.cooldownTime));
        return t;
    }

    // ========= 既存ヘルパ（Dash/Knockback 用）のラッパーもここに書ける =========

    public bool CanUseDash()
        => CanUse(SkillId.Dash);

    public bool DashHasStartAttack()
        => GetLevel(SkillId.Dash) >= 2;

    public bool DashHasEndAttack()
        => GetLevel(SkillId.Dash) >= 3;

    public bool CanUseKnockbackAttack()
        => CanUse(SkillId.KnockbackAttack);
}
