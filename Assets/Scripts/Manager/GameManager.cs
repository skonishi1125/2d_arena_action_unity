using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ゲーム中の現状体
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
    }

    private void Update()
    {
        if (State != GameState.Playing)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
            ClearGame();

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

        EndGame(false);
    }

}
