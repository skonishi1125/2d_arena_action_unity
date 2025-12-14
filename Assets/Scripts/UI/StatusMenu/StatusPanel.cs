using TMPro;
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI critText;
    [SerializeField] private TextMeshProUGUI evasionText;

    private EntityStatus status;
    private PlayerLevel level;
    private PlayerSkillController skill;

    public void Init(EntityStatus status, PlayerLevel level, PlayerSkillController skill)
    {
        this.status = status;
        this.level = level;
        this.skill = skill;

        RefreshAll();

        // レベルアップ時に自動で更新
        if (level != null)
        {
            level.OnLevelUp += _ => RefreshAll();
            level.OnExpChanged += (_, __) => RefreshAll();
        }

        // パッシブスキルで変わった場合
        if (skill != null)
            skill.OnStatusChangedBySkill += RefreshAll;

    }

    private void RefreshAll()
    {
        if (status == null || level == null) return;

        levelText.text = $"LV: {level.Level}";
        expText.text = $"EXP: {level.CurrentExp} / {level.CurrentRequiredExp}";
        hpText.text = $"HP: {status.GetMaxHp():0}";
        attackText.text = $"ATTACK: {status.GetAttack():0}";
        defenseText.text = $"DEFENSE: {status.GetDefense():0}";
        critText.text = $"CRITICAL: {(status.GetCritical() * 100f):0}%";
        evasionText.text = $"EVASION: {(status.GetEvasion() * 100f):0}%";
    }

    private void OnDestroy()
    {
        if (level != null)
        {
            level.OnLevelUp -= _ => RefreshAll();
            level.OnExpChanged -= (_, __) => RefreshAll();
        }

        if (skill != null)
            skill.OnStatusChangedBySkill -= RefreshAll;
    }

}
