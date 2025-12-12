using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotWidget : MonoBehaviour
{
    [SerializeField] private SkillSlot slot; // Z / D / V
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownMask;      // Filled想定
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Sprite emptySprite;

    private PlayerSkillController skill;
    private SkillId lastId = SkillId.None;

    public void Setup(PlayerSkillController skillController)
    {
        skill = skillController;
        Refresh(true);
    }

    private void Update()
    {
        if (skill == null) return;

        var id = skill.GetEquipped(slot);
        if (id != lastId)
        {
            Refresh(true);
        }

        if (lastId != SkillId.None && skill.GetLevel(lastId) > 0)
        {
            float ratio = skill.GetCooldownRatio(lastId);
            cooldownMask.fillAmount = 1f - ratio;
        }
    }

    private void Refresh(bool force)
    {
        lastId = skill.GetEquipped(slot);

        if (lastId == SkillId.None || skill.GetLevel(lastId) <= 0)
        {
            iconImage.sprite = emptySprite;
            levelText.text = "";
            cooldownMask.fillAmount = 0f;
            return;
        }

        var st = skill.GetState(lastId);
        iconImage.sprite = st.definition.icon;
        levelText.text = $"Lv.{skill.GetLevel(lastId)}";
        Debug.Log(lastId);
    }

}
