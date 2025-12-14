using DG.Tweening;
using TMPro;
using UnityEngine;
using static SkillButton;

public class SkillPanel : MonoBehaviour
{
    private PlayerLevel level;

    [Header("Skill Point UI")]
    [SerializeField] private TextMeshProUGUI spValueText;
    [SerializeField] private Color spZeroColor = Color.white;
    [SerializeField] private Color spPositiveColor = Color.yellow;

    [Header("Error Message UI")]
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private float fadeIn = 0.08f;
    [SerializeField] private float hold = 1.2f;
    [SerializeField] private float fadeOut = 0.35f;
    private Sequence errorSeq;

    private SkillButton[] cachedButtons;

    public void Init(PlayerLevel level)
    {
        CacheButtons();
        BindLevel(level);

        if (errorText == null)
            return;

        errorText.text = " ";

        // ロックImageの表示
        RefreshAllLockVisuals();
    }

    private void CacheButtons()
    {
        // SkillPanel配下の全ボタンをキャッシュ（Inactiveも含めたい場合 true）
        cachedButtons = GetComponentsInChildren<SkillButton>(true);
    }

    // 「同じスロットに別スキルが既に取得済みか」を判定
    public bool IsSlotOccupied(string slotKey, SkillId exceptSkillId)
    {
        if (cachedButtons == null || cachedButtons.Length == 0)
            CacheButtons();

        foreach (var b in cachedButtons)
        {
            if (b == null) continue;
            if (b.SkillId == exceptSkillId) continue;
            if (b.SlotKey != slotKey) continue;

            // Lv>0なら「取得済み」とみなす
            if (b.GetCurrentLevel() > 0)
                return true;
        }

        return false;
    }

    public void ShowError(SkillUpgradeFailReason reason)
    {
        string msg = reason switch
        {
            SkillUpgradeFailReason.NotEnoughSkillPoints => "スキルポイントが不足しています。",
            SkillUpgradeFailReason.SlotAlreadyOccupied => "同じスロットの技は複数取得できません。",
            _ => "取得できません。"
        };

        if (errorText == null)
            return;

        errorText.text = msg;

        // 複数連打されたときのため、既存の挙動を止めておく
        errorSeq?.Kill();

        // 表示→待機→フェードアウト
        errorText.alpha = 0f;
        errorSeq = DOTween.Sequence()
            .SetUpdate(true)// Timescaleに依存させない。
            .Append(errorText.DOFade(1f, fadeIn))
            .AppendInterval(hold)
            .Append(errorText.DOFade(0f, fadeOut))
            .OnComplete(() =>
            {
                // 完全に消えたら空白に戻す
                errorText.text = " ";
            });
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

    public void RefreshAllLockVisuals()
    {
        if (cachedButtons == null || cachedButtons.Length == 0)
            CacheButtons();

        foreach (var b in cachedButtons)
            b?.RefreshLockVisual();
    }

    private void OnDestroy()
    {
        if (level != null)
            level.OnSkillPointsChanged -= HandleSkillPointsChanged;
    }
}
