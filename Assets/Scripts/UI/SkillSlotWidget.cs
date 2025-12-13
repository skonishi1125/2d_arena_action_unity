using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotWidget : MonoBehaviour
{
    private PlayerSkillController skill;
    private SkillId slottedSkillId = SkillId.None;

    [SerializeField] private SkillSlot slot; // Z, D, VなどをInspector側で入れておく
    [SerializeField] private Image iconImage;// 元々白っぽい色。スキルを習得出来たらそのスキルアイコンに上書きする
    [SerializeField] private Image cooldownMask;
    [SerializeField] private TMP_Text levelText;
    // Lv.0用のアイコン
    // 今は白色だが、灰色にしたいケースなどが出た時は灰色を作ってセットすること。
    [SerializeField] private Sprite emptySprite;

    // クールタイム回復時のフラッシュ演出
    [SerializeField] private Image flashOverlay;
    [SerializeField] private float flashDuration = 0.12f;
    private bool wasOnCooldown; // 前フレームでCD中だったか
    private Coroutine playFlashCo;

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
        if (id != slottedSkillId)
            Refresh(true);

        // このスロットに格納されたスキルがNoneでなく、
        // レベルが1以上の時は記載の処理もUpdate()で行う
        if (slottedSkillId != SkillId.None && skill.GetLevel(slottedSkillId) > 0)
        {
            // レベルテキストの更新
            levelText.text = $"Lv.{skill.GetLevel(slottedSkillId)}";

            // クールタイムがある場合、そのアニメ更新
            float ratio = skill.GetCooldownRatio(slottedSkillId);
            cooldownMask.fillAmount = 1f - ratio;

            // クールタイムが回復した瞬間にフラッシュを入れて、wasOnCooldownを解除する
            bool isOnCooldown = ratio < 0.999f;
            if (wasOnCooldown && !isOnCooldown)
                PlayFlash();

            wasOnCooldown = isOnCooldown;

        }
    }

    private void Refresh(bool force)
    {
        // Inspector側で指定したキーを参考に、スロットのSkillIdを取得する
        slottedSkillId = skill.GetEquipped(slot);

        // Noneだったり、Levelが0だった場合は差し替え無し
        if (slottedSkillId == SkillId.None || skill.GetLevel(slottedSkillId) <= 0)
        {
            iconImage.sprite = emptySprite;
            levelText.text = "";
            cooldownMask.fillAmount = 0f;
            return;
        }

        // スロットに装備されているスキルの、スプライトに置き換える
        var state = skill.GetState(slottedSkillId);
        iconImage.sprite = state.definition.icon;
        levelText.text = $"Lv.{skill.GetLevel(slottedSkillId)}";
    }

    private void PlayFlash()
    {
        if (flashOverlay == null)
            return;

        if (playFlashCo != null)
            StopCoroutine(playFlashCo);
        playFlashCo = StartCoroutine(PlayFlashCo());
    }

    private IEnumerator PlayFlashCo()
    {
        float half = flashDuration * 0.5f;

        for (float t = 0; t < half; t += Time.deltaTime)
        {
            SetFlashAlpha(t / half);
            yield return null;
        }
        SetFlashAlpha(1f);

        // 1→0（半分の時間）
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            SetFlashAlpha(1f - (t / half));
            yield return null;
        }
        SetFlashAlpha(0f);

        playFlashCo = null;
    }

    private void SetFlashAlpha(float a)
    {
        var c = flashOverlay.color;
        c.a = a;
        flashOverlay.color = c;
    }

}
