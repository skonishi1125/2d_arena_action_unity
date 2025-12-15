using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    WaveIntro, // 3 2 1...と、カウントダウン
    Playing,
    LevelUp,
    GameOverSlowing,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ゲーム画面全体の状態
    public GameState State { get; private set; } = GameState.Ready;

    public Player Player { get; private set; }
    public UIWaveIntro WaveIntroUi { get; private set; }
    public UIBossAlert BossAlertUi { get; private set; }
    public UIResult ResultUi { get; private set; }
    public WaveManager WaveManager { get; private set; }

    private void Awake()
    {
        // Scene遷移したとき、そのSceneにGameManagerがあったときの配慮
        // これでDebug時、各シーンにGamemanagerを置いておける
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitGameForCurrentScene();
        if (!IsBattleScene())
            return;

        // BattleSceneなら、Player等の情報を保持するようにする
        CacheSceneObjects();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void CacheSceneObjects()
    {
        Player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(Player, nameof(Player), this))
            return;

        WaveIntroUi = FindFirstObjectByType<UIWaveIntro>();
        if (!LogHelper.AssertNotNull(WaveIntroUi, nameof(WaveIntroUi), this))
            return;

        BossAlertUi = FindFirstObjectByType<UIBossAlert>();
        if (!LogHelper.AssertNotNull(BossAlertUi, nameof(BossAlertUi), this))
            return;

        ResultUi = FindFirstObjectByType<UIResult>();
        if (!LogHelper.AssertNotNull(ResultUi, nameof(ResultUi), this))
            return;

        WaveManager = FindFirstObjectByType<WaveManager>();
        if (!LogHelper.AssertNotNull(WaveManager, nameof(WaveManager), this))
            return;

        CameraManager cameraManager = FindFirstObjectByType<CameraManager>();
        if (!LogHelper.AssertNotNull(cameraManager, nameof(CameraManager), this))
            return;

        cameraManager.Bind(Player.Health);

        // scene 再ロード時の二重登録防止のため、一度解除してから登録し直す
        Player.Level.OnLevelUp -= HandleLevelUp;
        Player.Health.OnDied -= SlowMotion;
        WaveManager.OnStageCleared -= HandleClearGame;
        WaveManager.OnBossWaveStarted -= HandleBossWaveStarted;
        WaveIntroUi.OnFinished -= HandleWaveIntroFinished;

        Player.Level.OnLevelUp += HandleLevelUp;
        Player.Health.OnDied += SlowMotion;
        WaveManager.OnStageCleared += HandleClearGame;
        WaveManager.OnBossWaveStarted += HandleBossWaveStarted;
        WaveIntroUi.OnFinished += HandleWaveIntroFinished;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitGameForCurrentScene();
        if (!IsBattleScene())
            return;

        // Player等を取り直す
        CacheSceneObjects();
    }

    private void InitGameForCurrentScene()
    {
        // 開始時、リトライ時共通の初期化
        Time.timeScale = 1f;
        State = GameState.Ready;

        // Battle シーンではロード後すぐにゲームを開始
        // 後でReadyを実装したとき、いろいろ挟むようにすればOK
        if (IsBattleScene())
        {
            CacheSceneObjects();
            StartGame();
        }
    }

    private bool IsBattleScene()
    {
        var name = SceneManager.GetActiveScene().name;
        return name.StartsWith("Battle"); // 例：BattleEasy / BattleNormal / BattleHard
    }

    private void HandleLevelUp(int newLevel)
    {
        // 例：スキル選択 State へ
        //State = GameState.LevelUp;

        // TODO: Lv UPUI を出す、入力を止める、など
        //Debug.Log($"Level Up! New Level: {newLevel}");
        //Time.timeScale = .01f;
        Player.Vfx.CreateOnLevelUpVfx(Player.transform);

    }

    private void Update()
    {

//        if (State == GameState.LevelUp)
//        {
//#if UNITY_EDITOR
//            Debug.Log("Levelup.. U を押すとPlayingに戻ります");
//            if (Input.GetKeyDown(KeyCode.U))
//            {
//                Time.timeScale = 1f;
//                State = GameState.Playing;
//            }
//#endif
//        }

    }
    public void StartGame()
    {
        State = GameState.WaveIntro;
        Debug.Log(WaveIntroUi);
        WaveIntroUi.Play();
    }

    private void HandleWaveIntroFinished()
    {
        // 他の状態から呼ばれた場合は無視
        if (State != GameState.WaveIntro)
            return;

        State = GameState.Playing;
        WaveManager.BeginStage();
    }

    private void HandleBossWaveStarted(WaveConfig wave)
    {
        if (State != GameState.Playing)
            return;

        BossAlertUi?.Play();
    }


    // Dieに、まずSlowMotionを購読させて呼び出す。
    // スローになって倒れた後、GameOverのSceneに遷移。
    private void SlowMotion()
    {
        if (State != GameState.Playing)
            return;

        StartCoroutine(SlowMotionCo());
    }
    private IEnumerator SlowMotionCo()
    {
        State = GameState.GameOverSlowing;
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;

        GameOver();
    }

    public void GameOver()
    {
        if (State != GameState.GameOverSlowing)
            return;

        EndGame(false);
    }

    // イベント購読用
    public void HandleClearGame()
    {
        ClearGame();
    }

    private void ClearGame()
    {
        // Playing中以外に呼ばれても、何もしない
        if (State != GameState.Playing)
            return;

        EndGame(true);
    }

    // Clear, GameOver共通処理
    private void EndGame(bool isClear)
    {
        State = GameState.Result;
        WaveManager?.StopStage();
        ResultUi.ShowResult(isClear);
    }

    public void RetryGame()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
