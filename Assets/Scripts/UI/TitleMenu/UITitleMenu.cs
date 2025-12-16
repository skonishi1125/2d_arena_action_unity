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
    [SerializeField] private AudioClip selectClip; // 操作SE
    [SerializeField] private AudioClip submitClip; // 決定SE

    private GameObject lastSelectedObject; // 直近で選ばれた選択肢
    private bool suppressNextSelectSfx;

    private void Awake()
    {
        HideAllModals();
    }

    private void HideAllModals()
    {
        dimmer.SetActive(false);
        difficultyModal.SetActive(false);
        quitConfirmModal.SetActive(false);
    }

    private void Start()
    {
        // ゲーム開始時の初期選択
        Focus(firstSelectedOnTitle, false); 
    }

    private void Update()
    {
        // EventSystemから選択中のObjectを監視しておく
        var es = EventSystem.current;
        if (es == null)
            return;

        var current = es.currentSelectedGameObject;
        if (current == null || current == lastSelectedObject)
            return;

        // 抑止フラグがfalseなら鳴らす。
        if (!suppressNextSelectSfx)
            AudioManager.Instance?.PlayUI(selectClip);

        // 初期選択以外は鳴らすので、固定でfalse
        suppressNextSelectSfx = false;
        lastSelectedObject = current;
    }

    // 引数で渡したGameObjectを初期選択状態とする
    // (ゲーム開始時のPlay, 難易度選択のTutorialなど）
    // boolの値で音を鳴らすか、鳴らさないかを指定できる
    // （デフォルトで指定されるボタンで、音が鳴らないようにするため。）
    private void Focus(GameObject target, bool playSelectSfx)
    {
        if (target == null || EventSystem.current == null)
            return;

        // Update()側でボタン変更時に音を鳴らすように監視している
        // Focusで選択させたときも同様に検知してSEをが鳴る。
        // falseを指定したとき（鳴らさない）、抑止フラグをtrueとして立てる
        // trueのとき（鳴らすとき）、抑止フラグはfalseとしておけばよい
        suppressNextSelectSfx = !playSelectSfx;
        lastSelectedObject = target;

        // 念のためクリアしてから改めてセットしておく
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }

    // ========= UIのボタン関連 ========= 

    public void OnClickPlay()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        ShowModal(difficultyModal);
        Focus(firstSelectedOnDifficulty, false);
    }

    public void OnClickQuit()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        ShowModal(quitConfirmModal);
        Focus(firstSelectedOnQuitConfirm, false);
    }

    public void OnClickCloseModal()
    {
        AudioManager.Instance?.PlayUI(submitClip);
        HideAllModals();
        Focus(firstSelectedOnTitle, false);
    }

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

    private void LoadBattle(string sceneName)
    {
        HideAllModals();
        SceneManager.LoadScene(sceneName);
    }

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

    private void ShowModal(GameObject modal)
    {
        dimmer.SetActive(true);
        difficultyModal.SetActive(modal == difficultyModal);
        quitConfirmModal.SetActive(modal == quitConfirmModal);
    }




}
