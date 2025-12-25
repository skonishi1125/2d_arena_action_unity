using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    [Header("Description Row")]
    [SerializeField] private GameObject descriptionRow;

    [Header("TipBar")]
    [SerializeField] private TipBar tipBar;
    private bool disableTip;

    [Header("Top")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI masterLevelHeaderText;
    [SerializeField] private TextMeshProUGUI summaryText;

    [Header("Requirement")]
    [SerializeField] private GameObject requirementLevelRow;
    [SerializeField] private TextMeshProUGUI levelValueText; // 例: SPValueText → LevelValueText

    // "-------" はObject側で静的な文字列を入れて管理

    [Header("Details")]
    [SerializeField] private TextMeshProUGUI currentLevelHeaderText;  // 「[現在レベル:3]」など
    [SerializeField] private TextMeshProUGUI currentLevelBodyText;    // 現在レベルの説明
    [SerializeField] private TextMeshProUGUI nextLevelHeaderText;     // 「[次のレベル:4]」
    [SerializeField] private TextMeshProUGUI nextLevelBodyText;       // 次のレベルの説明
    [SerializeField] private GameObject nextLevelGroup;               // Max時は非表示

    private void Awake()
    {
        descriptionRow.SetActive(false);

        // シーン名で一度だけ判定
        var sceneName = SceneManager.GetActiveScene().name;
        disableTip = (sceneName == "BattleHard");
    }

    public void Show(SkillDefinition def, int currentLevel)
    {
        if (def == null)
        {
            // 何も指定がないときは、説明チップを出す
            ShowDefaultHelp();
            return;
        }

        descriptionRow.SetActive(true);

        if (requirementLevelRow == null || levelValueText == null)
            return;

        // 次に上げるレベル（未習得なら1、習得済みなら+1）
        int nextLevel = (currentLevel <= 0) ? 1 : currentLevel + 1;
        var nextData = def.GetLevelData(nextLevel);
        if (nextData != null)
            levelValueText.text = nextData.minPlayerLevel.ToString();
        else
            levelValueText.text = def.GetLevelData(currentLevel).minPlayerLevel.ToString();

        // 上部
        skillIcon.sprite = def.icon;
        skillNameText.text = def.displayName;
        masterLevelHeaderText.text = $"[マスターレベル:{def.MaxLevel}]";
        summaryText.text = def.description;

        // --- Lv0（未習得）の場合 ---
        if (currentLevel <= 0)
        {
            currentLevelHeaderText.text = "[現在レベル: 未習得]";
            // 好きな文言でよい
            currentLevelBodyText.text = "まだこのスキルを習得していません。";

            // Next は Lv1 を表示
            var next = def.GetLevelData(1);
            if (next != null)
            {
                nextLevelGroup.SetActive(true);
                nextLevelHeaderText.text = "[次のレベル:1]";
                nextLevelBodyText.text = next.levelDescription;
            }
            else
            {
                nextLevelGroup.SetActive(false);
            }

            return;
        }

        // --- Lv1 以上の通常ケース ---
        var current = def.GetLevelData(currentLevel);
        if (current != null)
        {
            currentLevelHeaderText.text = $"[現在レベル:{currentLevel}]";
            currentLevelBodyText.text = current.levelDescription;
        }
        else
        {
            currentLevelHeaderText.text = $"[現在レベル:{currentLevel}]";
            currentLevelBodyText.text = string.Empty;
        }

        if (currentLevel < def.MaxLevel)
        {
            var next = def.GetLevelData(currentLevel + 1);
            if (next != null)
            {
                nextLevelGroup.SetActive(true);
                nextLevelHeaderText.text = $"[次のレベル:{currentLevel + 1}]";
                nextLevelBodyText.text = next.levelDescription;
            }
            else
            {
                nextLevelGroup.SetActive(false);
            }
        }
        else
        {
            // Max のときは「次のレベル」行を消す
            nextLevelGroup.SetActive(false);
        }
    }

    public void ShowDefaultHelp()
    {
        descriptionRow.SetActive(true);

        // 上部：適当に「未選択」状態を表現
        skillIcon.sprite = null;
        skillNameText.text = "HELP";
        masterLevelHeaderText.text = "";
        summaryText.text =
            "LvUPしたら [ESC] でスキル選択\n" +
            "Physical / Magic は排他\n" +
            "Passive は基礎ステータスを永続強化";

        currentLevelHeaderText.text = "";
        currentLevelBodyText.text = "";

        nextLevelGroup.SetActive(false);
    }

    public void Hide()
    {
        descriptionRow.SetActive(false);
    }

    // TipBar操作用のAPI
    public void ShowTip(float seconds = 4f)
    {
        if (disableTip)
            return;
        if (tipBar == null)
           return;

        tipBar.Show(seconds);
    }

    public void HideTip()
    {
        if (disableTip)
            return;

        if (tipBar == null)
            return;

        tipBar.Hide();
    }

}
