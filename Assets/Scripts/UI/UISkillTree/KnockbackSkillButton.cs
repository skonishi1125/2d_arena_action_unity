using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class KnockbackSkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Player player;

    [SerializeField] private PlayerSkillController playerSkill;
    [SerializeField] private TextMeshProUGUI levelText;        // ボタン上に表示するレベル
    [SerializeField] private GameObject descriptionPanel;      // 説明用パネル
    [SerializeField] private TextMeshProUGUI descriptionText;  // 説明テキスト

    [TextArea]
    [SerializeField]
    private string description =
        "KB Lv1: パワーKB習得\nKB Lv2: パワーKB\nKB Lv3: 強化パワーKB";


    private void Awake()
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

    private void UpdateView()
    {
        if (playerSkill == null || levelText == null)
            return;

        int lv = playerSkill.GetLevel(SkillId.KnockbackAttack);
        levelText.text = $"KB Lv.{lv}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(true);
            if (descriptionText != null)
                descriptionText.text = description;
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

        playerSkill.LevelUp(SkillId.KnockbackAttack);
        UpdateView();
    }
}
