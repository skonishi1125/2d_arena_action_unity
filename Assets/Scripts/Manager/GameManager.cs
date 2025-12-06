using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    LevelUp,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Level, Health等を取得するためにPlayerを持たせる
    [SerializeField] public Player player;

    // ゲーム中の現状態
    public GameState State { get; private set; } = GameState.Ready;
    [SerializeField] private float waveDuration = 30f; // この秒数生き残ればクリア
    private float remainingTime;

    // スポナー
    [SerializeField] private EnemySpawner enemySpawner;

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
        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        StartGame();
        player.Level.OnLevelUp += HandleLevelUp;
        player.Health.OnDied += GameOver;
    }
    private void OnDestroy()
    {
        player.Level.OnLevelUp -= HandleLevelUp;
        player.Health.OnDied -= GameOver;
    }

    private void Update()
    {

        if (State == GameState.LevelUp)
        {
            Debug.Log("Levelup.. U を押すとPlayingに戻ります");
            if (Input.GetKeyDown(KeyCode.U))
            {
                Time.timeScale = 1f;
                State = GameState.Playing;
            }
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
        State = GameState.Playing;
        remainingTime = waveDuration;
        enemySpawner.BeginSpawn();
    }

    private void ClearGame()
    {
        // Playing中以外に呼ばれても、何もしない
        if (State != GameState.Playing)
            return;

        EndGame(true);
    }

    private void EndGame(bool isClear)
    {
        State = GameState.Result;
        enemySpawner.StopSpawn();

        // TODO: UI表示など
        Debug.Log(isClear ? "CLEAR!" : "GAME OVER");

        // 後で：タイトルに戻る・リトライ etc.
    }
    public void GameOver()
    {
        if (State != GameState.Playing)
            return;

        Time.timeScale = 0.5f;
        EndGame(false);
    }

    private void HandleLevelUp(int newLevel)
    {
        // 例：スキル選択 State へ
        State = GameState.LevelUp;

        // TODO: Lv UPUI を出す、入力を止める、など
        Debug.Log($"Level Up! New Level: {newLevel}");
        Time.timeScale = .01f;

    }

}
