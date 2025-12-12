using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotWidget : MonoBehaviour
{
    [SerializeField] private SkillSlot slot; // Z, D, VなどをInspector側で入れておく
    [SerializeField] private Image iconImage;// 元々白っぽい色。スキルを習得出来たらそのスキルアイコンに上書きする
    [SerializeField] private Image cooldownMask;
    [SerializeField] private TMP_Text levelText;
    // Lv.0用のアイコン
    // 今は白色だが、灰色にしたいケースなどが出た時は灰色を作ってセットすること。
    [SerializeField] private Sprite emptySprite;

    private PlayerSkillController skill;
    private SkillId lastId = SkillId.None;

    // UIInGame などから、ゲーム開始時に呼ばれる
    public void Setup(PlayerSkillController skillController)
    {
        skill = skillController;
        Refresh(true);
    }

    private void Update()
    {
        // skillControllerの情報がないなら、何もしない
        if (skill == null)
            return;

        // 装備中のSkillIdを取得
        // もしLevelUpでスキルスロットに装備されたら、こちらが反応する
        // リフレッシュ処理を実行
        var id = skill.GetEquipped(slot);
        if (id != lastId)
            Refresh(true);

        if (lastId != SkillId.None && skill.GetLevel(lastId) > 0)
        {
            levelText.text = $"Lv.{skill.GetLevel(lastId)}";
            float ratio = skill.GetCooldownRatio(lastId);
            cooldownMask.fillAmount = 1f - ratio;
        }
    }

    private void Refresh(bool force)
    {
        // Inspector側で指定したキーを参考に、スロットのSkillIdを取得する
        lastId = skill.GetEquipped(slot);

        // Noneだったり、Levelが0だった場合は差し替え無し
        if (lastId == SkillId.None || skill.GetLevel(lastId) <= 0)
        {
            iconImage.sprite = emptySprite;
            levelText.text = "";
            cooldownMask.fillAmount = 0f;
            return;
        }

        // スロットに装備されているスキルの、スプライトに置き換える
        var state = skill.GetState(lastId);
        iconImage.sprite = state.definition.icon;
        levelText.text = $"Lv.{skill.GetLevel(lastId)}";
    }

}
