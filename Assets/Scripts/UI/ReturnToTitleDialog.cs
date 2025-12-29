using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ReturnToTitleDialog : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject dimmer;
    [SerializeField] private GameObject dialogRoot;

    [Header("Selection")]
    [SerializeField] private GameObject firstSelectedOnOpen; // Yesボタンなど

    [Header("Scene")]
    [SerializeField] private string titleSceneName = "Title";
    private void Awake()
    {
        Hide();
    }
    public void Show()
    {
        dimmer.SetActive(true);
        dialogRoot.SetActive(true);

        // UI操作の初期選択（ゲームパッド/キー操作のため）
        if (firstSelectedOnOpen != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedOnOpen);
        }
    }

    public void Hide()
    {
        dialogRoot.SetActive(false);
        dimmer.SetActive(false);
    }

    // Yes
    public void OnClickYes()
    {
        Time.timeScale = 1f; // もしポーズで止めている可能性があるなら
        SceneManager.LoadScene(titleSceneName);
    }

    // No
    public void OnClickNo()
    {
        Hide();
    }
}
