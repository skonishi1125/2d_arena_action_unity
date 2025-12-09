using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Player player;

    [SerializeField] private PlayerSkillController playerSkill;
    [SerializeField] private TextMeshProUGUI levelText; // ボタン上部のtext
    [SerializeField] private GameObject descriptionPanel; // 説明用パネルの割当
    [SerializeField] private TextMeshProUGUI descriptionText;  // パネル子要素のtext
    [TextArea]
    [SerializeField]
    private string description = "TODO";

    [Header("Cooldown UI")]
    [SerializeField] private Image cooldownMask;

    // 子要素のボタンが操作するスキルID
    protected abstract SkillId TargetSkillId { get; }


    protected virtual void Awake()
    {
        // Prefab運用想定のため、PlayerはFindFirstObjectByTypeで取得
        player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(player, nameof(player), this))
            return;

        playerSkill = player.GetComponentInChildren<PlayerSkillController>();
        if (!LogHelper.AssertNotNull(playerSkill, nameof(playerSkill), this))
            return;

    }
    private void Start()
    {
        // 最初に表示を更新
        UpdateView();

        // 説明パネルはデフォルト非表示
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
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
        float ratio = playerSkill.GetCooldownRatio(TargetSkillId);

        // マスクは覆っている量なので、逆にする
        float cover = 1f - ratio;
        cooldownMask.fillAmount = cover;

        // 完全に準備完了のときだけマスクを消す（好み）
        cooldownMask.enabled = cover > 0f;
    }

    private void UpdateView()
    {
        if (playerSkill == null || levelText == null)
            return;

        int lv = playerSkill.GetLevel(TargetSkillId);
        levelText.text = GetLevelLabel(lv);
    }

    protected virtual string GetLevelLabel(int level)
    {
        return $"Lv.{level}";
    }
    protected virtual string GetDescription()
    {
        return description;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(true);
            if (descriptionText != null)
                descriptionText.text = GetDescription();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerSkill == null)
            return;

        // 基本動作：対象スキルのレベルアップ
        if (playerSkill.LevelUp(TargetSkillId))
            UpdateView();
    }
}
