using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Player player;

    [SerializeField] private PlayerSkillController playerSkill;
    private Image skillIconImage;

    [Header("Definition")]
    [SerializeField] private SkillDefinition skillDefinition;  // このボタンが表すスキル

    [Header("Description UI")]
    [SerializeField] private DescriptionPanel descriptionPanel;

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

        // ImageにSkillDefinitionに定義したImageを割り当てる
        skillIconImage = GetComponent<Image>();
        skillIconImage.sprite = skillDefinition.icon;
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
        float ratio = playerSkill.GetCooldownRatio(TargetSkillId);

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

        int lv = playerSkill.GetLevel(TargetSkillId);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel == null || playerSkill == null || skillDefinition == null)
            return;

        int lv = playerSkill.GetLevel(TargetSkillId);
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
        if (playerSkill == null)
            return;

        // スキルのレベルを上げた時
        if (playerSkill.LevelUp(TargetSkillId))
        {
            UpdateView();

            // パネル表示中なら、説明も更新
            if (descriptionPanel != null && skillDefinition != null)
            {
                int lv = playerSkill.GetLevel(TargetSkillId);
                descriptionPanel.Show(skillDefinition, lv);
            }
        }
    }
}
