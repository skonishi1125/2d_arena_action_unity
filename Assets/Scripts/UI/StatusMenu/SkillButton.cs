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

    [Header("Definition")]
    [SerializeField] private SkillDefinition skillDefinition;  // このボタンが表すスキル
    [SerializeField] private SkillId skillId;

    [Header("Description UI")]
    [SerializeField] private DescriptionPanel descriptionPanel;

    [Header("Cooldown UI")]
    [SerializeField] private Image cooldownMask;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelText;  // アイコンの上
    [SerializeField] private TextMeshProUGUI keyText;    // アイコンの下
    [SerializeField] private string keyLabel;

    [Header("Behavior")]
    [SerializeField] private bool canLevelUpOnClick = true;// クリックでレベルアップするか
    [SerializeField] private bool showDescriptionOnHover = true; // ホバーで説明を出すか

    [SerializeField] private SkillPanel skillPanel;
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

    }
    private void Start()
    {
        UpdateView();
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
    }

    // マウスを外しても、表示のままでいいかも。
    public void OnPointerExit(PointerEventData eventData)
    {
        //if (descriptionPanel != null)
        //    descriptionPanel.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canLevelUpOnClick)
            return;

        int currentLv = GetCurrentLevel();

        // 「未取得」かつ「同スロットに別スキル取得済み」なら弾く
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

        // レベルアップ成功時
        if (playerSkill.LevelUp(skillId))
        {
            playerLevel.TrySpendSkillPoints(skillCost);
            UpdateView();

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
}
