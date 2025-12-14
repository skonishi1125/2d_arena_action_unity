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

    // 親のUIStatusMenuから実行してもらう
    // PlayerLevelが渡されるので、以下を実行
    // * 自身の持つスキルボタンの所持(chysicならそれを, magicならそれを。）
    // * PlayerLevelのSP増減イベントの購読(SPの更新に必要。）
    // * エラーテキストを空白に(disableとすると、表示したときにレイアウトがずれる)
    // * スキルにlockのグレー枠を被せる
    public void Init(PlayerLevel level)
    {
        CacheButtons();
        BindLevel(level);

        if (errorText == null)
            return;

        errorText.text = " ";

        RefreshAllLockVisuals();
    }

    // xxxRowの持つ全スキルボタンの取得（Inactiveも含めるためtrue）
    private void CacheButtons()
    {
        cachedButtons = GetComponentsInChildren<SkillButton>(true);
    }

    // スキルポイント増減イベントへの購読
    // パネルにcurrent SPを表示する場所があるので、それを動的に更新するために必要
    private void BindLevel(PlayerLevel newLevel)
    {
        if (level != null)
            level.OnSkillPointsChanged -= HandleSkillPointsChanged;

        level = newLevel;

        if (level != null)
        {
            level.OnSkillPointsChanged += HandleSkillPointsChanged;

            // 初回更新（イベント待ちだと初回が更新されないため）
            HandleSkillPointsChanged(level.SkillPoints);
        }
    }

    // 各ボタンでlockOverrayを表示させるかどうかの判断をする
    public void RefreshAllLockVisuals()
    {
        if (cachedButtons == null || cachedButtons.Length == 0)
            CacheButtons();

        foreach (var b in cachedButtons)
            b?.RefreshLockVisual();
    }

    // 同じスロットに別スキルが既に取得済みかどうかを返す
    // スキルクリック時にこのメソッドで判断する
    public bool IsSlotOccupied(string slotKey, SkillId exceptSkillId)
    {
        if (cachedButtons == null || cachedButtons.Length == 0)
            CacheButtons();

        // 各ボタンで、IDを持っているか、スロットに値があるか
        // レベルが1以上かどうかを確認
        foreach (var button in cachedButtons)
        {
            if (button == null)
                continue;
            if (button.SkillId == exceptSkillId)
                continue;
            if (button.SlotKey != slotKey)
                continue;

            if (button.GetCurrentLevel() > 0)
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

    // パネルに出す現在所持中のSP表示を管理する
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
