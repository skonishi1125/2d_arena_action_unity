using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DashSkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Player player;

    [SerializeField] private PlayerSkillController playerSkill;
    [SerializeField] private TextMeshProUGUI levelText;        // ボタン上に表示するレベル
    [SerializeField] private GameObject descriptionPanel;      // 説明用パネル
    [SerializeField] private TextMeshProUGUI descriptionText;  // 説明テキスト

    [TextArea]
    [SerializeField]
    private string description =
        "Dash Lv1: ダッシュ習得\nDash Lv2: ダッシュ攻撃\nDash Lv3: 強化ダッシュ";


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

        int lv = playerSkill.DashSkillLevel;
        levelText.text = $"Dash Lv.{lv}";
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

        playerSkill.LevelUpDash();
        UpdateView();
    }
}
