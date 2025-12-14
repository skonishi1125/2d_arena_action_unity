using System.Collections;
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
    [SerializeField] private TextMeshProUGUI errorText;   // 画像の赤文字Textを割り当て
    [SerializeField] private float errorHideSeconds = 2.0f;

    private Coroutine hideErrorRoutine;
    private SkillButton[] cachedButtons;

    public void Init(PlayerLevel level)
    {
        CacheButtons();
        BindLevel(level);
        HideErrorImmediate();
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
        if (errorText == null) return;

        string msg = reason switch
        {
            SkillUpgradeFailReason.NotEnoughSkillPoints =>
                "スキルポイントが不足しています。",
            SkillUpgradeFailReason.SlotAlreadyOccupied =>
                "同じスロットの技は複数取得できません。",
            _ =>
                "この操作は実行できません。"
        };

        errorText.gameObject.SetActive(true);
        errorText.text = msg;

        if (hideErrorRoutine != null)
            StopCoroutine(hideErrorRoutine);

        hideErrorRoutine = StartCoroutine(HideErrorAfterSeconds(errorHideSeconds));
    }

    private IEnumerator HideErrorAfterSeconds(float seconds)
    {
        // メニュー中は timeScale = 0なので、realtime
        yield return new WaitForSecondsRealtime(seconds);
        HideErrorImmediate();
    }

    private void HideErrorImmediate()
    {
        if (errorText == null)
            return;

        //errorText.gameObject.SetActive(false);
        errorText.text = " ";
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
