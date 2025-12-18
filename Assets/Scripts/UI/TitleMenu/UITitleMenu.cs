using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;


public class UITitleMenu : MonoBehaviour
{
    [Header("Intro Animation (DOTween)")]
    [SerializeField] private RectTransform titleTextRect;
    [SerializeField] private CanvasGroup titleTextCanvasGroup;

    [SerializeField] private CanvasGroup pressEnterCanvasGroup;

    [SerializeField] private Vector2 titleStartOffset = new Vector2(0f, 40f);
    [SerializeField] private float titleFadeDuration = 0.6f;
    [SerializeField] private float titleMoveDuration = 0.6f;

    [SerializeField] private float pressBlinkDuration = 0.6f;
    [SerializeField, Range(0f, 1f)] private float pressBlinkMinAlpha = 0.25f;

    private Vector2 _titleBasePos;
    private Tween _pressBlinkTween;

    // PRESS ENTER
    [SerializeField] private GameObject pressEnterRoot;
    [SerializeField] private GameObject mainMenuRoot;

    [Header("Overlays")]
    [SerializeField] private GameObject dimmer;
    [SerializeField] private GameObject dimmerMessage;
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

    private InputAction _submitAction; // press to enter用
    private InputAction _cancelAction;

    private bool _isPressEnterPhase = true;

    [Header("Loading Overlay")]
    [SerializeField] private GameObject loadingRoot;
    [SerializeField] private TMP_Text loadingPointText; // LoadingPoint の TMP
    [SerializeField] private float loadingPointInterval = 0.1f;
    [SerializeField] private int loadingPointMax = 3;

    private bool isLoading;
    private Coroutine LoadBattleAsyncCo;


    private void Awake()
    {
        HideAllModals();

        if (loadingRoot != null)
            loadingRoot.SetActive(false);
    }

    private void HideAllModals()
    {
        dimmer.SetActive(false);
        dimmerMessage.SetActive(false);
        difficultyModal.SetActive(false);
        quitConfirmModal.SetActive(false);
    }

    private void Start()
    {
        HideAllModals();

        pressEnterRoot.SetActive(true);
        mainMenuRoot.SetActive(false);

        // PressEnter待ち中は「選択なし」にしておく
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        lastSelectedObject = null;
        _isPressEnterPhase = true;


        // 開始時にタイトルを出す。(DOTween)
        PlayIntro();

    }

    private void PlayIntro()
    {
        // タイトル：初期位置を記録
        if (titleTextRect != null)
            _titleBasePos = titleTextRect.anchoredPosition;

        // タイトル：上から少し下がりつつフェードイン
        if (titleTextCanvasGroup != null)
            titleTextCanvasGroup.alpha = 0f;

        if (titleTextRect != null)
            titleTextRect.anchoredPosition = _titleBasePos + titleStartOffset;

        var seq = DOTween.Sequence();

        if (titleTextCanvasGroup != null)
            seq.Join(titleTextCanvasGroup.DOFade(1f, titleFadeDuration).SetEase(Ease.OutQuad));

        if (titleTextRect != null)
            seq.Join(titleTextRect.DOAnchorPos(_titleBasePos, titleMoveDuration).SetEase(Ease.OutCubic));

        // PRESS ENTER：点滅（Yoyo）
        if (pressEnterCanvasGroup != null)
        {
            pressEnterCanvasGroup.alpha = 1f;

            _pressBlinkTween?.Kill();
            _pressBlinkTween = pressEnterCanvasGroup
                .DOFade(pressBlinkMinAlpha, pressBlinkDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
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

        // 初回Focus以外の場合のケース
        // Easyを選んでキャンセルしたとき、targetが黄色のままになっているのを防ぐ
        if (lastSelectedObject != null)
        {
            var component = lastSelectedObject.GetComponent<MenuButtonHighlight>();

            if (component != null)
                component.Apply(false);

        }

        lastSelectedObject = target;

        // 念のためクリアしてから改めてセットしておく
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);

    }

    private void OnEnable()
    {
        // EventSystem の UI Input Module から Cancel Action を取って購読する
        // esがあって、esのCancelに振られたキーがあるときの挙動
        if (EventSystem.current != null &&
            EventSystem.current.TryGetComponent<InputSystemUIInputModule>(out var ui))
        {
            if (ui.cancel != null)
            {
                _cancelAction = ui.cancel.action;
                _cancelAction.performed += OnCancelPerformed;
                _cancelAction.Enable();
            }

            if (ui.submit != null)
            {
                _submitAction = ui.submit.action;
                _submitAction.performed += OnSubmitPerformed;
                _submitAction.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (_cancelAction != null)
            _cancelAction.performed -= OnCancelPerformed;

        if (_submitAction != null)
            _submitAction.performed -= OnSubmitPerformed;
    }

    private void OnSubmitPerformed(InputAction.CallbackContext _)
    {
        if (!_isPressEnterPhase)
            return;

        AudioManager.Instance?.PlayUI(submitClip);

        // 点滅停止
        _pressBlinkTween?.Kill();

        pressEnterRoot.SetActive(false);
        mainMenuRoot.SetActive(true);

        _isPressEnterPhase = false;

        // PRESS ENTER時は選択しない
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // 次フレームにフォーカス（＝同じEnterでPlayが押されない）
        StartCoroutine(FocusNextFrame(firstSelectedOnTitle, false));

    }
    private IEnumerator FocusNextFrame(GameObject target, bool playSelectSfx)
    {
        yield return null;
        Focus(target, playSelectSfx);
    }

    private void OnCancelPerformed(InputAction.CallbackContext _)
    {
        // モーダルが開いてる時だけ閉じる（Title通常時は何もしない）
        if (difficultyModal.activeSelf || quitConfirmModal.activeSelf)
            OnClickCloseModal(); // 既存の「閉じる」処理を再利用

        // Focusの色替えを止める
        Debug.Log("cancel");

    }

    // ========= UIのボタン関連 ========= 

    public void OnClickPlay()
    {
        Debug.Log("OnClickPlay");
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

    // battleシーン移行時の処理
    private void LoadBattle(string sceneName)
    {
        if (isLoading)
            return;

        if (LoadBattleAsyncCo != null)
            StopCoroutine(LoadBattleAsyncCo);

        LoadBattleAsyncCo = StartCoroutine(LoadBattleAsync(sceneName));
    }

    private IEnumerator LoadBattleAsync(string sceneName)
    {
        isLoading = true;

        // モーダルを閉じ、関連するオブジェクトのactiveを全てfalse
        HideAllModals();
        if (mainMenuRoot != null)
            mainMenuRoot.SetActive(false);

        if (pressEnterRoot != null)
            pressEnterRoot.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        lastSelectedObject = null;

        // Loading表示ON
        if (loadingRoot != null)
            loadingRoot.SetActive(true);

        if (loadingPointText != null)
            loadingPointText.text = "";

        // 1フレーム待って「Loading...」を確実に描画してからロード開始
        yield return null;

        var op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null)
        {
            isLoading = false;
            if (loadingRoot != null) loadingRoot.SetActive(false);
            yield break;
        }

        int dots = 0;
        float timer = 0f;

        while (!op.isDone)
        {
            timer += Time.unscaledDeltaTime;

            if (timer >= loadingPointInterval)
            {
                timer = 0f;
                dots = (dots % loadingPointMax) + 1;

                if (loadingPointText != null)
                    loadingPointText.text = new string('.', dots);
            }

            yield return null;
        }
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
        OnClickCloseModal();
    }

    private void ShowModal(GameObject modal)
    {
        dimmer.SetActive(true);
        dimmerMessage.SetActive(true);
        difficultyModal.SetActive(modal == difficultyModal);
        quitConfirmModal.SetActive(modal == quitConfirmModal);
    }




}
