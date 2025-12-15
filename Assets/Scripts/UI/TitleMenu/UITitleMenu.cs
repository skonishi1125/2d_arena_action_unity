using UnityEngine;
using UnityEngine.SceneManagement;

public class UITitleMenu : MonoBehaviour
{
    [Header("Overlays")]
    [SerializeField] private GameObject dimmer;
    [SerializeField] private GameObject difficultyModal;
    [SerializeField] private GameObject quitConfirmModal;
    private void Awake()
    {
        HideAllModals();
    }

    public void OnClickPlay()
    {
        ShowModal(difficultyModal);
    }

    public void OnClickQuit()
    {
        ShowModal(quitConfirmModal);
    }

    public void OnClickCloseModal()
    {
        HideAllModals();
    }

    // Difficulty buttons
    public void OnChooseTutorial() => LoadBattle("BattleTutorial");
    public void OnChooseEasy() => LoadBattle("BattleEasy");
    public void OnChooseNormal() => LoadBattle("BattleNormal");
    public void OnChooseHard() => LoadBattle("BattleHard");

    // Quit confirm
    public void OnQuitYes()
    {
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnQuitNo()
    {
        HideAllModals();
    }

    private void LoadBattle(string sceneName)
    {
        HideAllModals();
        SceneManager.LoadScene(sceneName);
    }

    private void ShowModal(GameObject modal)
    {
        dimmer.SetActive(true);
        difficultyModal.SetActive(modal == difficultyModal);
        quitConfirmModal.SetActive(modal == quitConfirmModal);
    }

    private void HideAllModals()
    {
        dimmer.SetActive(false);
        difficultyModal.SetActive(false);
        quitConfirmModal.SetActive(false);
    }


}
