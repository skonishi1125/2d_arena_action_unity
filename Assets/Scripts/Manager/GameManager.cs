using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    WaveIntro, // 3 2 1...と、カウントダウン
    Playing,
    Slowing,
    Result
}

public enum GameOverCause
{
    None = 0,
    PlayerDied,
    ObjectiveDestroyed,
    // Future: TimeUp, etc...
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ゲーム画面全体の状態
    public GameState State { get; private set; } = GameState.Ready;
    // ゲームオーバー理由
    private GameOverCause pendingGameOverCause = GameOverCause.None;

    public Player Player { get; private set; }
    public Objective Objective { get; private set; }
    public UIWaveIntro WaveIntroUi { get; private set; }
    public UIBossAlert BossAlertUi { get; private set; }
    public UIResult ResultUi { get; private set; }
    public UIClearFlash ClearFlash { get; private set; }
    public WaveManager WaveManager { get; private set; }

    // Player等の初回キャッシュフラグ
    private int _initializedSceneHandle = -1;

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
        HandleSceneChanged(SceneManager.GetActiveScene());
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Additiveロード等でアクティブシーンが変わってないなら何もしない
        if (scene != SceneManager.GetActiveScene())
            return;

        HandleSceneChanged(scene);
    }

    private void HandleSceneChanged(Scene scene)
    {
        StopAllCoroutines();

        // Startでのキャッシュ、Battleシーン遷移時のキャッシュの2重処理を防ぐ
        if (_initializedSceneHandle == scene.handle)
            return;
        _initializedSceneHandle = scene.handle;

        Time.timeScale = 1f;
        State = GameState.Ready;

        if (!scene.name.StartsWith("Battle"))
        {
            // Battle外：Playerが存在する保証がないので外しておく
            Enemy.OnExpGained -= AddExp;
            Player = null;
            WaveManager = null;
            WaveIntroUi = null;
            BossAlertUi = null;
            ResultUi = null;
            return;
        }

        if (!CacheSceneObjects())
        {
            // キャッシュが失敗したら、1フレーム後に再試行
            Debug.Log("キャッシュ失敗したので再試行");
            StartCoroutine(RetryCacheNextFrame(scene));
            return;
        }

        _initializedSceneHandle = scene.handle;

        ApplyPlayerInput();// キャッシュして、Playerが取れてから切り替える
        StartGame();
    }

    private IEnumerator RetryCacheNextFrame(Scene scene)
    {
        yield return null;
        // まだ同じアクティブシーンなら再試行
        if (scene == SceneManager.GetActiveScene())
            HandleSceneChanged(scene);
    }

    private void ApplyPlayerInput()
    {
        if (Player == null) return;
        Player.input.UI.Disable();
        Player.input.Player.Enable();
    }

    //private void ApplyUIInput()
    //{
    //    if (Player == null) return;
    //    Player.input.Player.Disable();
    //    Player.input.UI.Enable();
    //}

    private bool CacheSceneObjects()
    {
        Player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(Player, nameof(Player), this))
            return false;

        Objective = FindFirstObjectByType<Objective>();
        if (!LogHelper.AssertNotNull(Objective, nameof(Objective), this))
            return false;

        WaveIntroUi = FindFirstObjectByType<UIWaveIntro>();
        if (!LogHelper.AssertNotNull(WaveIntroUi, nameof(WaveIntroUi), this))
            return false;

        BossAlertUi = FindFirstObjectByType<UIBossAlert>();
        if (!LogHelper.AssertNotNull(BossAlertUi, nameof(BossAlertUi), this))
            return false;

        ResultUi = FindFirstObjectByType<UIResult>();
        if (!LogHelper.AssertNotNull(ResultUi, nameof(ResultUi), this))
            return false;

        ClearFlash = FindFirstObjectByType<UIClearFlash>();
        if (!LogHelper.AssertNotNull(ClearFlash, nameof(ClearFlash), this))
            return false;

        WaveManager = FindFirstObjectByType<WaveManager>();
        if (!LogHelper.AssertNotNull(WaveManager, nameof(WaveManager), this))
            return false;

        CameraManager cameraManager = FindFirstObjectByType<CameraManager>();
        if (!LogHelper.AssertNotNull(cameraManager, nameof(CameraManager), this))
            return false;

        // キャッシュがうまくいったときは、購読を進める

        cameraManager.BindPlayerHealth(Player.Health);
        cameraManager.BindObjectiveHealth(Objective.Health);

        // scene 再ロード時の二重登録防止のため、一度解除してから登録し直す
        Player.Level.OnLevelUp -= HandleLevelUp;

        // ============= ラムダ購読解除（学習メモを書いとく）
        // コメントアウトした書き方だと、線用メソッドでなく使い捨てメソッド(ラムダ)として購読している
        // なので、度のメソッドを購読したのか or 解除したのかが分からないので、解除できない
        // HandleXXXとしておくと、HandleXXXの 購読 or 解除 ができるようになる
        // なので、購読するときは購読専用のメソッドを作っておくとよい
        Player.Health.OnDied -= HandlePlayerDied;
        Objective.Health.OnDestroyed -= HandleObjectiveDestroyed;
        // Player.Health.OnDied -= () => TriggerGameOver(GameOverCause.PlayerDied);
        // Objective.Health.OnDestroyed -= _ => TriggerGameOver(GameOverCause.ObjectiveDestroyed);

        WaveManager.OnStageCleared -= TriggerGameClear;
        WaveManager.OnBossWaveStarted -= HandleBossWaveStarted;
        WaveIntroUi.OnFinished -= HandleWaveIntroFinished;
        Enemy.OnExpGained -= AddExp;

        // ============= ラムダ購読の開始
        Player.Level.OnLevelUp += HandleLevelUp;

        Player.Health.OnDied += HandlePlayerDied;
        Objective.Health.OnDestroyed += HandleObjectiveDestroyed;
        //Player.Health.OnDied += () => TriggerGameOver(GameOverCause.PlayerDied);
        //Objective.Health.OnDestroyed += _ => TriggerGameOver(GameOverCause.ObjectiveDestroyed);

        WaveManager.OnStageCleared += TriggerGameClear;
        WaveManager.OnBossWaveStarted += HandleBossWaveStarted;
        WaveIntroUi.OnFinished += HandleWaveIntroFinished;
        Enemy.OnExpGained += AddExp;

        return true;
    }

    private void HandlePlayerDied()
    {
        TriggerGameOver(GameOverCause.PlayerDied);
    }

    private void HandleObjectiveDestroyed(ObjectiveHealth _)
    {
        TriggerGameOver(GameOverCause.ObjectiveDestroyed);
    }


    private void HandleLevelUp(int newLevel)
    {
        if (Player == null)
        {
            Debug.LogWarning("GameManager:HandleLevelUp(): Playerがnullです。");
            return;
        }
        Player.Vfx.CreateOnLevelUpVfx(Player.transform);
    }

    public void StartGame()
    {
        State = GameState.WaveIntro;
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

    private void AddExp(int exp)
    {
        if (Player == null)
        {
            Debug.LogWarning("GameManager:AddExp(): Playerがnullです。");
            return;
        }
        Player.Level.AddExp(exp);
    }

    private void HandleBossWaveStarted(WaveConfig wave)
    {
        if (State != GameState.Playing)
            return;

        BossAlertUi?.Play();
    }


    private void TriggerGameOver(GameOverCause cause)
    {
        if (State != GameState.Playing)
            return;

        pendingGameOverCause = cause;
        StartCoroutine(SlowMotionCo(false));
    }

    public void GameOver()
    {
        if (State != GameState.Slowing)
            return;

        EndGame(false, pendingGameOverCause);
    }

    public void TriggerGameClear()
    {
        ClearFlash.Play(); // フラッシュ演出
        StartCoroutine(SlowMotionCo(true));
    }

    private void GameClear()
    {
        if (State != GameState.Slowing)
            return;

        EndGame(true);
    }

    // GameOver, GameClear時のスロー演出
    private IEnumerator SlowMotionCo(bool isGameClear)
    {
        State = GameState.Slowing;
        AudioManager.Instance?.StopBgm();
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;

        // スローが終わってから、各結果に移動
        if (isGameClear)
            GameClear();
        else
            GameOver();
    }

    // Clear, GameOver共通処理
    private void EndGame(bool isClear, GameOverCause cause = GameOverCause.None)
    {
        State = GameState.Result;
        WaveManager?.StopStage();
        ResultUi.ShowResult(isClear, cause);
    }

    public void RetryGame()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
