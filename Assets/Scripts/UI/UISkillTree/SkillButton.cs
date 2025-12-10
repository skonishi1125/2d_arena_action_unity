using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Player player;

    [SerializeField] private PlayerSkillController playerSkill;
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
        if (skillIconImage != null && skillDefinition != null)
            skillIconImage.sprite = skillDefinition.icon;

        // キーテキストは一度固定で設定
        if (keyText != null)
            keyText.text = keyLabel; // 例: "Z", "D", "V" など

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
        // ゲーム中に出ているスキルアイコンなどはfalseとすることで、
        // クリックしてもレベルアップ処理が働かないようにしておく。
        if (!canLevelUpOnClick)
            return;

        if (playerSkill == null)
            return;

        // スキルのレベルを上げた時
        if (playerSkill.LevelUp(skillId))
        {
            UpdateView();

            // パネル表示中なら、説明も更新
            if (descriptionPanel != null && skillDefinition != null)
            {
                int lv = playerSkill.GetLevel(skillId);
                descriptionPanel.Show(skillDefinition, lv);
            }
        }
    }
}
