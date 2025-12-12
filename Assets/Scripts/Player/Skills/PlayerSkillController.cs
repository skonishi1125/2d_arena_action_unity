using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Skill States (Definition + Runtime)")]
    [SerializeField]
    private SkillRuntimeState[] skillStates;

    private readonly Dictionary<SkillId, SkillRuntimeState> skillMap
        = new Dictionary<SkillId, SkillRuntimeState>();

    // キーボードの枠に度のスキルが入っているか。
    private readonly Dictionary<SkillSlot, SkillId> equipped = new();
    public SkillId GetEquipped(SkillSlot slot)
    => equipped.TryGetValue(slot, out var id) ? id : SkillId.None;

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

    // スキルスロット枠に、そのスキルを装備する
    public void Equip(SkillId id)
    {
        var st = GetState(id);
        if (st == null || st.definition == null) return;

        var def = st.definition;
        if (def.slot == SkillSlot.None) return;

        if (def.exclusiveInSlot)
            equipped[def.slot] = id;   // ここで排他（上書き）
    }

    // ========= 基本取得まわり =========
    // PlayerSkillControllerに設定されたDataを元に、
    // スキルの現在状況を取得して返す。クールタイムや現時点のレベルなど
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

    // 呼び出された時点のスキルデータを返す
    public SkillLevelData GetCurrentLevelData(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return null;

        return state.CurrentLevelData;
    }

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

        // 初回習得の場合、排他チェック
        // 既存のスロット技を覚えている場合は弾く。
        if (state.currentLevel == 0 && state.definition.exclusiveInSlot)
        {
            var slot = state.definition.slot;
            var equippedId = GetEquipped(slot);
            if (equippedId != SkillId.None && equippedId != id)
                return false;
        }

        if (!CanLevelUp(id))
            return false;

        state.currentLevel++;
        Debug.Log($"Skill {id} leveled up to {state.currentLevel}");

        // 初回習得の場合、枠を確定
        if (state.currentLevel == 1 && state.definition.exclusiveInSlot)
            Equip(id);

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
        => CanUse(SkillId.HeavyKB);
}
