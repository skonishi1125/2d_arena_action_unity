using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResult : MonoBehaviour
{
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button returnToTitleButton;

    private void Awake()
    {
        // UIResultの子要素全てをまとめる、rootPanelだけをfalseにする
        // これによって、UIResult自体はActiveになるので、
        // GameManagerなどが  FindFirstObjectByType<UIResult>(); で探索できるようになる。
        rootPanel.SetActive(false);
    }

    public void ShowResult(bool isClear)
    {
        rootPanel.SetActive(true);
        resultText.text = isClear ? "GAME CLEAR!" : "EXHAUSTED!";

        retryButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RetryGame();
        });

        returnToTitleButton.onClick.AddListener(() =>
        {
            // タイトル画面がないので暫定
            Debug.Log("Titleへ");
        });
    }
}
