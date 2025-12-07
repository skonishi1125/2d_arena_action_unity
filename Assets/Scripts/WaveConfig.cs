using UnityEngine;

public enum WaveClearType
{
    SurviveTime,   // 一定時間生き延びる
    KillAll,       // 相手を全滅
    KillBoss,      // ボス撃破(敵が残っていても、倒せばOK)
}

[CreateAssetMenu(menuName = "Game/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public string waveName;

    public float startDelay;       // Wave開始前の待ち時間（演出用）
    public EnemyGroup[] enemyGroups;

    // Wave クリア条件
    public WaveClearType clearType;

    // SurviveTime 用
    public float surviveDuration = 30f;
    public bool isBossWave;
}

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int spawnCount;         // 何体出すか
    public float spawnInterval;    // 何秒おきに出すか
}
