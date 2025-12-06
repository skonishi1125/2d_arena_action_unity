using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    LevelUp,
    GameOverSlowing,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ゲーム中の状態
    public GameState State { get; private set; } = GameState.Ready;
    [SerializeField] private float waveDuration = 30f; // この秒数生き残ればクリア
    private float remainingTime;

    // Level, Health等を取得するためにPlayerを持たせる
    public Player Player { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }
    public UIResult UIResult { get; private set; }

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
        CacheSceneObjects();
        InitGameForCurrentScene();
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
        EnemySpawner = FindFirstObjectByType<EnemySpawner>();
        UIResult = FindFirstObjectByType<UIResult>();

        // ★ CameraManager の再バインド
        var cameraManager = FindFirstObjectByType<CameraManager>();
        if (cameraManager != null && Player != null)
            cameraManager.Bind(Player.Health);

        if (Player != null)
        {
            // scene 再ロード時の二重登録防止のため、一度解除してから登録し直す
            Player.Level.OnLevelUp -= HandleLevelUp;
            Player.Health.OnDied -= SlowMotion;

            Player.Level.OnLevelUp += HandleLevelUp;
            Player.Health.OnDied += SlowMotion;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Player等を取り直す
        CacheSceneObjects();
        InitGameForCurrentScene();
    }

    private void InitGameForCurrentScene()
    {
        // 開始時、リトライ時共通の初期化
        Time.timeScale = 1f;
        remainingTime = waveDuration;
        State = GameState.Ready;

        // Battle シーンではロード後すぐにゲームを開始
        // 後でReadyを実装したとき、いろいろ挟むようにすればOK
        var scene = SceneManager.GetActiveScene();
        if (scene.name == "BattleEasy")
        {
            StartGame();
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        // 例：スキル選択 State へ
        State = GameState.LevelUp;

        // TODO: Lv UPUI を出す、入力を止める、など
        Debug.Log($"Level Up! New Level: {newLevel}");
        Time.timeScale = .01f;

    }

    private void Update()
    {

        if (State == GameState.LevelUp)
        {
#if UNITY_EDITOR
            Debug.Log("Levelup.. U を押すとPlayingに戻ります");
            if (Input.GetKeyDown(KeyCode.U))
            {
                Time.timeScale = 1f;
                State = GameState.Playing;
            }
#endif
        }

        if (State == GameState.Playing)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
                ClearGame();
        }

    }
    public void StartGame()
    {
        if (EnemySpawner == null)
            return;

        State = GameState.Playing;
        remainingTime = waveDuration;
        EnemySpawner.BeginSpawn();
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
        EnemySpawner.StopSpawn();

        Debug.Assert(UIResult != null, "[GameManager] UIResult が見つかりません");
        // ゲームクラッシュ対策
        if (UIResult == null)
            return;

        UIResult.ShowResult(isClear);
    }

    public void RetryGame()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
