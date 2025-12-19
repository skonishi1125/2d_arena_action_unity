using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    WaveIntro, // 3 2 1...と、カウントダウン
    Playing,
    GameOverSlowing,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ゲーム画面全体の状態
    public GameState State { get; private set; } = GameState.Ready;

    public Player Player { get; private set; }
    public Objective Objective { get; private set; }
    public UIWaveIntro WaveIntroUi { get; private set; }
    public UIBossAlert BossAlertUi { get; private set; }
    public UIResult ResultUi { get; private set; }
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
        // Startでのキャッシュ、Battleシーン遷移時のキャッシュの2重処理を防ぐ
        if (_initializedSceneHandle == scene.handle)
            return;
        _initializedSceneHandle = scene.handle;

        Time.timeScale = 1f;
        State = GameState.Ready;

        if (!scene.name.StartsWith("Battle"))
        {
            // Battle外：Playerが存在する保証がないので、ここで切替はしない（またはガード付き）
            Enemy.OnExpGained -= AddExp;

            // もし「直前のBattleのPlayer参照が残っている」ケースだけ安全に落とすなら
            if (Player != null)
            {
                Player.input.Player.Disable();
                Player.input.UI.Disable();
            }

            Player = null;
            WaveManager = null;
            WaveIntroUi = null;
            BossAlertUi = null;
            ResultUi = null;
            return;
        }

        CacheSceneObjects();
        ApplyPlayerInput();// キャッシュして、Playerが取れてから切り替える
        StartGame();
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

    private void CacheSceneObjects()
    {
        Debug.Log("CacheSceneObjects");

        Player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(Player, nameof(Player), this))
            return;

        Objective = FindFirstObjectByType<Objective>();
        if (!LogHelper.AssertNotNull(Objective, nameof(Objective), this))
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
        Objective.Health.OnDestroyed -= _ => SlowMotion();
        WaveManager.OnStageCleared -= HandleClearGame;
        WaveManager.OnBossWaveStarted -= HandleBossWaveStarted;
        WaveIntroUi.OnFinished -= HandleWaveIntroFinished;
        Enemy.OnExpGained -= AddExp;

        Player.Level.OnLevelUp += HandleLevelUp;
        Player.Health.OnDied += SlowMotion;
        Objective.Health.OnDestroyed += _ => SlowMotion();
        WaveManager.OnStageCleared += HandleClearGame;
        WaveManager.OnBossWaveStarted += HandleBossWaveStarted;
        WaveIntroUi.OnFinished += HandleWaveIntroFinished;
        Enemy.OnExpGained += AddExp;
    }


    private void HandleLevelUp(int newLevel)
    {
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
