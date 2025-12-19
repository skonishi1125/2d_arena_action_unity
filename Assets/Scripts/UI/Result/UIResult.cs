using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIResult : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button returnToTitleButton;

    [SerializeField] private GameObject firstSelectedResultButton;

    private void Awake()
    {
        // UIResultの子要素全てをまとめる、rootPanelだけをfalseにする
        // これによって、UIResult自体はActiveになるので、
        // GameManagerなどが  FindFirstObjectByType<UIResult>(); で探索できるようになる。
        rootPanel.SetActive(false);

        player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(player, nameof(player), this))
            return;

    }

    public void ShowResult(bool isClear)
    {
        // InputSetをUI用に変える
        player.input.Player.Disable();
        player.input.UI.Enable();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedResultButton);

        var component = firstSelectedResultButton.GetComponent<ResultButtonHighlight>();

        if (component != null)
            component.Apply(true);

        rootPanel.SetActive(true);
        resultText.text = isClear ? "GAME CLEAR!" : "EXHAUSTED!";

        //retryButton.onClick.AddListener(() =>
        //{
        //    GameManager.Instance.RetryGame();
        //});

        //returnToTitleButton.onClick.AddListener(() =>
        //{
        //    // タイトル画面がないので暫定
        //    Debug.Log("Titleへ");
        //});
    }

    // リトライボタンに割り当てる
    public void RetryGame()
    {
        GameManager.Instance.RetryGame();
    }

    // タイトルボタンに割り当てる

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }


}
