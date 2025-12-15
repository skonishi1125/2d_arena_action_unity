using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UITitleMenu : MonoBehaviour
{
    [Header("Overlays")]
    [SerializeField] private GameObject dimmer;
    [SerializeField] private GameObject difficultyModal;
    [SerializeField] private GameObject quitConfirmModal;

    // キーボード入力するときの、初期選択の指定
    [Header("First Selection")]
    [SerializeField] private GameObject firstSelectedOnTitle;
    [SerializeField] private GameObject firstSelectedOnDifficulty;
    [SerializeField] private GameObject firstSelectedOnQuitConfirm;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip selectClip;
    [SerializeField] private AudioClip submitClip;

    private GameObject _lastSelected;
    private bool _suppressNextSelectSfx;


    private void Awake()
    {
        HideAllModals();
    }

    private void Start()
    {
        Focus(firstSelectedOnTitle, playSelectSfx: false); // 初期選択は鳴らさない
    }

    private void Update()
    {
        var es = EventSystem.current;
        if (es == null) return;

        var current = es.currentSelectedGameObject;
        if (current == null || current == _lastSelected) return;

        if (!_suppressNextSelectSfx)
        {
            AudioManager.Instance?.PlayUI(selectClip);
        }

        _suppressNextSelectSfx = false;
        _lastSelected = current;
    }

    private void Focus(GameObject target, bool playSelectSfx)
    {
        if (target == null || EventSystem.current == null) return;

        _suppressNextSelectSfx = !playSelectSfx;
        _lastSelected = target;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }


    public void OnClickPlay()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        ShowModal(difficultyModal);
        Focus(firstSelectedOnDifficulty, playSelectSfx: false);
    }

    public void OnClickQuit()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        ShowModal(quitConfirmModal);
        Focus(firstSelectedOnQuitConfirm, playSelectSfx: false);
    }

    public void OnClickCloseModal()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        HideAllModals();
        Focus(firstSelectedOnTitle, playSelectSfx: false);
    }

    // Difficulty buttons
    public void OnChooseTutorial()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        LoadBattle("BattleTutorial");
    }

    public void OnChooseEasy()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        LoadBattle("BattleEasy");
    }

    public void OnChooseNormal()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        LoadBattle("BattleNormal");
    }
    public void OnChooseHard()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        LoadBattle("BattleHard");
    }

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
