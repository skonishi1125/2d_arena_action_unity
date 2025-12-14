using TMPro;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    private PlayerLevel level;

    [Header("Skill Point UI")]
    [SerializeField] private TextMeshProUGUI spValueText;
    [SerializeField] private Color spZeroColor = Color.white;
    [SerializeField] private Color spPositiveColor = Color.yellow;

    public void Init(PlayerLevel level)
    {
        BindLevel(level);

    }

    private void BindLevel(PlayerLevel newLevel)
    {
        if (level != null)
            level.OnSkillPointsChanged -= HandleSkillPointsChanged;

        level = newLevel;

        if (level != null)
        {
            level.OnSkillPointsChanged += HandleSkillPointsChanged;

            // 初期表示（イベント待ちだと初回が更新されないため）
            HandleSkillPointsChanged(level.SkillPoints);
        }
    }
    private void HandleSkillPointsChanged(int sp)
    {
        if (spValueText == null)
            return;

        spValueText.text = sp.ToString();
        spValueText.color = (sp <= 0) ? spZeroColor : spPositiveColor;
    }

    private void OnDestroy()
    {
        if (level != null)
            level.OnSkillPointsChanged -= HandleSkillPointsChanged;
    }
}
