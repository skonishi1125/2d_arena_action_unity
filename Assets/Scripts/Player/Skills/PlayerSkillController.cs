using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    private PlayerLevel playerLevel;
    // パッシブ適用のために使う
    private EntityStatus entityStatus;

    [Header("Skill States (Definition + Runtime)")]
    [SerializeField]
    private SkillRuntimeState[] skillStates;

    // 指定したSkillIdの状態を知るために使う
    // Lv, 定義など
    private readonly Dictionary<SkillId, SkillRuntimeState> skillMap
        = new Dictionary<SkillId, SkillRuntimeState>();

    // Z,D,Vのスキル枠に何が入っているかの確認
    // equipped["Z"] = 3 みたいに、連想配列みたいなイメージで使える
    private readonly Dictionary<SkillSlot, SkillId> equipped = new();

    // パッシブ用 ステータス変化時に呼ぶアクション
    // UIのステータス更新用途など。
    public event Action OnStatusChangedBySkill;


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

        entityStatus = GetComponent<EntityStatus>();
        if (!LogHelper.AssertNotNull(entityStatus, nameof(entityStatus), this))
            return;

        playerLevel = GetComponent<PlayerLevel>();
        if (!LogHelper.AssertNotNull(playerLevel, nameof(playerLevel), this))
            return;

    }

    private void Update()
    {
        UpdateCooldowns(Time.deltaTime);
    }

    // スロットに何のSkillIdが格納されているかを返す
    public SkillId GetEquipped(SkillSlot slot)
        => equipped.TryGetValue(slot, out var id) ? id : SkillId.None;

    // スキルスロット枠に、そのスキルを装備する
    public void Equip(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return;

        // 指定したSkillIdの、定義の確認
        // スロットが無指定のスキル（まだ未実装だが）の場合は、何もしない
        var def = state.definition;
        if (def.slot == SkillSlot.None)
            return;

        // 枠を占有するスキルかどうかの確認
        // 該当した場合、equipped[Z] = Dash というような形で入る
        // ※同枠のスキルが来た時格納されてしまうが、LevelUp()で弾いている
        if (def.exclusiveInSlot)
            equipped[def.slot] = id;
    }

    // ========= 基本取得まわり =========

    // PlayerSkillControllerに設定されたDataを元に、
    // 引数で指定したスキルの現在状況を取得して返す（クールタイムや現時点のレベルなど）
    // 全ての処理の基本。
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

    public bool CanLevelUp(SkillId id)
    {
        var state = GetState(id);
        if (state == null || state.definition == null)
            return false;

        return state.currentLevel < state.MaxLevel;
    }

    public bool LevelUp(SkillId id)
    {
        // 存在するスキルかどうかの確認
        var state = GetState(id);
        if (state == null || state.definition == null)
            return false;

        // 排他チェック
        // 初回習得 ( level0 )かつ、そのスキルが占有スキルの場合
        if (state.currentLevel == 0 && state.definition.exclusiveInSlot)
        {
            var slot = state.definition.slot; // ZかDか...等
            var equippedId = GetEquipped(slot); // スロットに格納されているSkillId

            // スロットがNoneでない場合（何かが入っている場合）
            // && スロットのIDが今回の対象スキルと違う場合, 弾く
            if (equippedId != SkillId.None && equippedId != id)
                return false;
        }

        // 最大レベルの場合は弾く
        if (!CanLevelUp(id))
            return false;

        // Playerのレベルが満たしているか
        int nextLevel = state.currentLevel + 1;
        var nextData = state.definition.GetLevelData(nextLevel);
        if (nextData == null)
            return false;

        if (playerLevel != null && playerLevel.Level < nextData.minPlayerLevel)
            return false;

        int oldLevel = state.currentLevel;
        state.currentLevel++;
        int newLevel = state.currentLevel;

        // パッシブ適用
        ApplyPassiveDelta(state, oldLevel, newLevel);
        OnStatusChangedBySkill?.Invoke();
        // UIInGameのヘルスバーも更新が必要

        // 初回習得の場合、枠を確定
        if (state.currentLevel == 1 && state.definition.exclusiveInSlot)
            Equip(id);

        return true;
    }

    // パッシブで上昇するステータスの適用
    // delta: 差分ということ
    private void ApplyPassiveDelta(SkillRuntimeState state, int oldLevel, int newLevel)
    {
        if (entityStatus == null)
            return;

        var def = state.definition;
        if (def == null)
            return;

        var newData = def.GetLevelData(newLevel);
        if (newData == null || !newData.isPassive)
            return; // アクティブスキルはここで弾く。

        var oldData = def.GetLevelData(oldLevel);

        // 差分の確認と適用
        ApplyPassiveDeltaByModifiers(oldData, newData);
    }

    // スキルレベルアップ前後を比較して、差分を足す
    // 10% -> 15%とそのまま上げていくと、25%とどんどん上がっていくので。
    // ex) SLv1で10%, SLv2で15%なら、5%分引き上げる
    private void ApplyPassiveDeltaByModifiers(SkillLevelData oldData, SkillLevelData newData)
    {
        if (entityStatus == null)
            return;

        if (newData == null || !newData.isPassive || newData.passiveModifiers == null)
            return;

        // newDataに書かれている modifier を基準に差分を計算して適用する
        foreach (var mod in newData.passiveModifiers)
        {
            var status = GetStatusRef(mod.param);
            if (status == null) continue;

            float oldValue = FindTotalValue(oldData, mod.param, mod.mode);
            float newValue = FindTotalValue(newData, mod.param, mod.mode);
            float delta = newValue - oldValue;

            if (Mathf.Approximately(delta, 0f))
                continue;

            if (mod.mode == ModifyMode.AddBonus)
                status.AddBonus(delta);
            else
                status.AddMultiplier(delta);
        }
    }


    // SkillDefinition側で定義する、
    // 「何のパラメータを上げるのか」という情報を取ってくる
    private static float FindTotalValue(SkillLevelData data, StatusParam param, ModifyMode mode)
    {
        if (data == null || !data.isPassive || data.passiveModifiers == null)
            return 0f;

        float sum = 0f;
        foreach (var m in data.passiveModifiers)
        {
            if (m.param == param && m.mode == mode)
                sum += m.value;
        }
        return sum;
    }

    private Status GetStatusRef(StatusParam param)
    {
        return param switch
        {
            StatusParam.MaxHp => entityStatus.maxHp,
            StatusParam.Attack => entityStatus.attack,
            StatusParam.Defense => entityStatus.defense,
            StatusParam.Evasion => entityStatus.evasion,
            StatusParam.Critical => entityStatus.critical,
            _ => null
        };
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

    // UI 用。
    // 1 = クールダウン完了、0 = 使った直後
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

    public bool DashHasStartAttack()
        => GetLevel(SkillId.Dash) >= 2;

    public bool DashHasEndAttack()
        => GetLevel(SkillId.Dash) >= 4;
}
