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
    private PlayerHealth health;
    private PlayerLevel level;
    private PlayerSkillController skill;
    private PlayerTimedModifiers timed;



    public void Init(
        EntityStatus status, PlayerHealth health, PlayerLevel level,
        PlayerSkillController skill, PlayerTimedModifiers timed
    )
    {
        this.status = status;
        this.health = health;
        this.level = level;
        this.skill = skill;
        this.timed = timed;

        RefreshAll();

        // ダメージ / 回復時に自動更新
        if (health != null)
            health.OnHealthUpdate += RefreshAll;

        // レベルアップ時に自動で更新
        if (level != null)
        {
            level.OnLevelUp += _ => RefreshAll();
            level.OnExpChanged += (_, __) => RefreshAll();
        }

        // パッシブスキルで変わった場合
        if (skill != null)
            skill.OnStatusChangedBySkill += RefreshAll;

        // アイテムで変わった場合
        if (timed != null)
            timed.OnStatusChangedByItem += RefreshAll;

    }

    private void RefreshAll()
    {
        if (status == null || level == null)
            return;

        levelText.text = $"Player Lv: {level.Level}";
        expText.text = $"EXP: {level.CurrentExp} / {level.CurrentRequiredExp}";
        hpText.text = $"HP: {health.GetCurrentHp()} / {status.GetMaxHp():0}";
        attackText.text = $"ATTACK: {status.GetAttack():0}";
        defenseText.text = $"DEFENSE: {status.GetDefense():0}";
        critText.text = $"CRITICAL: {(status.GetCritical() * 100f):0}%";
        evasionText.text = $"EVASION: {(status.GetEvasion() * 100f):0}%";
    }

    private void OnDestroy()
    {
        // TODO: 購読が消えていない懸念がある
        if (level != null)
        {
            level.OnLevelUp -= _ => RefreshAll();
            level.OnExpChanged -= (_, __) => RefreshAll();
        }

        if (skill != null)
            skill.OnStatusChangedBySkill -= RefreshAll;

        if (timed != null)
            timed.OnStatusChangedByItem -= RefreshAll;
    }

}
