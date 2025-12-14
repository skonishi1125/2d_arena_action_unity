using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const int skillCost = 1; // スキル獲得、LvUpに必要なSP

    private Player player;
    private PlayerSkillController playerSkill;
    private PlayerLevel playerLevel;
    private Image skillIconImage;
    [SerializeField] private SkillPanel skillPanel;


    [Header("Definition")]
    [SerializeField] private SkillDefinition skillDefinition;  // このボタンが表すスキル
    [SerializeField] private SkillId skillId;

    [Header("Description UI")]
    [SerializeField] private DescriptionPanel descriptionPanel;

    [Header("Cooldown UI")]
    [SerializeField] private Image cooldownMask;

    [Header("LockOverlay")]
    [SerializeField] private Image lockOverlay;

    [Header("HoverFrame")]
    [SerializeField] private Image hoverFrame;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelText;  // アイコンの上
    [SerializeField] private TextMeshProUGUI keyText;    // アイコンの下
    [SerializeField] private string keyLabel;

    [Header("Behavior")]
    [SerializeField] private bool canLevelUpOnClick = true;// クリックでレベルアップするか
    [SerializeField] private bool showDescriptionOnHover = true; // ホバーで説明を出すか

    public enum SkillUpgradeFailReason
    {
        NotEnoughSkillPoints,
        SlotAlreadyOccupied,
    }

    public SkillId SkillId => skillId;
    public string SlotKey => keyLabel;


    protected virtual void Awake()
    {
        // Prefab運用想定のため、PlayerはFindFirstObjectByTypeで取得
        player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(player, nameof(player), this))
            return;

        playerSkill = player.GetComponentInChildren<PlayerSkillController>();
        if (!LogHelper.AssertNotNull(playerSkill, nameof(playerSkill), this))
            return;

        playerLevel = player.GetComponentInChildren<PlayerLevel>();
        if (!LogHelper.AssertNotNull(playerLevel, nameof(playerLevel), this))
            return;

        // ImageにSkillDefinitionに定義したImageを割り当てる
        skillIconImage = GetComponent<Image>();
        if (skillIconImage != null && skillDefinition != null)
            skillIconImage.sprite = skillDefinition.icon;

        // キーテキストは一度固定で設定
        if (keyText != null)
            keyText.text = keyLabel; // 例: "Z", "D", "V" など

        if (skillPanel == null)
            skillPanel = GetComponentInParent<SkillPanel>();

        // SP増減イベントの購読
        playerLevel.OnSkillPointsChanged += HandleSpChanged;

    }


    private void Start()
    {
        UpdateView();
        RefreshLockVisual();
    }

    private void Update()
    {
        UpdateCooldownMask();
    }


    // スキル使用後、暗い表記から回復していく描写
    private void UpdateCooldownMask()
    {
        if (cooldownMask == null || playerSkill == null)
            return;

        // PlayerSkillController から 0〜1 の割合をもらう
        // ratio = 1: クール完了 / 0: 使った直後
        float ratio = playerSkill.GetCooldownRatio(skillId);

        // マスクは覆っている量なので、逆にする
        float cover = 1f - ratio;
        cooldownMask.fillAmount = cover;

        // 完全に準備完了のときだけマスクを消す（好み）
        cooldownMask.enabled = cover > 0f;
    }

    private void UpdateView()
    {
        if (playerSkill == null)
            return;

        int lv = playerSkill.GetLevel(skillId);
        
        if (levelText != null)
        {
            if (lv <= 0)
                levelText.text = "";
            else
                levelText.text = $"Lv {lv}";
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!showDescriptionOnHover)
            return;

        if (descriptionPanel == null || playerSkill == null || skillDefinition == null)
            return;

        int lv = playerSkill.GetLevel(skillId);
        descriptionPanel.Show(skillDefinition, lv);

        // アンロックの場合は、外枠を光らせない
        if (lockOverlay.enabled)
            return;

        if (hoverFrame == null)
            return;

        hoverFrame.enabled = true;
        hoverFrame.color = new Color(1f, 1f, 0f, 0f);
        hoverFrame.DOFade(1f, 0.15f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // アンロックの場合は何もしない
        if (lockOverlay.enabled)
            return;

        if (hoverFrame == null)
            return;

        hoverFrame.DOFade(0f, 0.15f)
            .SetUpdate(true)
            .OnComplete(() => hoverFrame.enabled = false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canLevelUpOnClick)
            return;

        int currentLv = GetCurrentLevel();

        // 「未取得」かつ「同スロットに別スキル取得済み」の場合
        if (currentLv <= 0 && skillPanel != null && skillPanel.IsSlotOccupied(SlotKey, skillId))
        {
            skillPanel.ShowError(SkillUpgradeFailReason.SlotAlreadyOccupied);
            return;
        }

        // SP不足
        if (playerLevel.SkillPoints < skillCost)
        {
            skillPanel?.ShowError(SkillUpgradeFailReason.NotEnoughSkillPoints);
            return;
        }

        // 問題なければスキルレベルアップ処理。
        if (playerSkill.LevelUp(skillId))
        {
            // スキルポイント減算処理
            playerLevel.TrySpendSkillPoints(skillCost);
            UpdateView();

            // 他のボタンも含めてロック状態を更新
            // スロット占有した場合、別スキルのロック状態にも影響がある
            skillPanel?.RefreshAllLockVisuals();

            if (descriptionPanel != null && skillDefinition != null)
            {
                int lv = playerSkill.GetLevel(skillId);
                descriptionPanel.Show(skillDefinition, lv);
            }
        }
    }

    // SkillPanelが参照するための現在Lv取得口
    public int GetCurrentLevel()
    {
        if (playerSkill == null) return 0;
        return playerSkill.GetLevel(skillId);
    }

    // スキルロックの表示をイベント購読するためのメソッド
    private void HandleSpChanged(int sp) => RefreshLockVisual();

    public void RefreshLockVisual()
    {
        if (lockOverlay == null || playerLevel == null || playerSkill == null)
            return;

        int lv = GetCurrentLevel();

        bool locked = false;

        if (lv <= 0)
        {
            if (playerLevel.SkillPoints < skillCost) // SP不足
                locked = true;
            else if (skillPanel != null && skillPanel.IsSlotOccupied(SlotKey, skillId)) // スロット重複
                locked = true;
        }

        lockOverlay.enabled = locked;
    }

    private void OnDestroy()
    {
        if (playerLevel != null)
            playerLevel.OnSkillPointsChanged -= HandleSpChanged;
    }
}
