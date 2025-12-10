using TMPro;
using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{
    [Header("Top")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI masterLevelHeaderText;
    [SerializeField] private TextMeshProUGUI summaryText;

    // "-------" はObject側で静的な文字列を入れて管理

    [Header("Details")]
    [SerializeField] private TextMeshProUGUI currentLevelHeaderText;  // 「[現在レベル:3]」など
    [SerializeField] private TextMeshProUGUI currentLevelBodyText;    // 現在レベルの説明
    [SerializeField] private TextMeshProUGUI nextLevelHeaderText;     // 「[次のレベル:4]」
    [SerializeField] private TextMeshProUGUI nextLevelBodyText;       // 次のレベルの説明
    [SerializeField] private GameObject nextLevelGroup;               // Max時は非表示

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(SkillDefinition def, int currentLevel)
    {
        if (def == null) return;

        gameObject.SetActive(true);

        // 上部
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

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
