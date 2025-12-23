using UnityEngine;

public enum WaveClearType
{
    SurviveTime,   // 一定時間生き延びる
    KillAll,       // 相手を全滅
    KillBoss,      // ボス撃破(敵が残っていても、倒せばOK)
}

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int spawnCount;         // 何体出すか
    public float spawnInterval;    // 何秒おきに出すか
    public EnemyRole enemyRole;    // Wanderer or Raider
    public bool countForWaveClear = true; // Wanderer は false
}

[System.Serializable]
public class EnemyStatMultiplier
{
    public float hp = 1f;   // 1.0 = 等倍
    public float atk = 1f;
    public float def = 1f;
    public float eva = 1f;
    public float crit = 1f;

    public float exp = 1f; // 1.0 = 等倍

    public static EnemyStatMultiplier One => new EnemyStatMultiplier
    {
        hp = 1f,
        atk = 1f,
        def = 1f,
        eva = 1f,
        crit = 1f
    };
}

[CreateAssetMenu(menuName = "Game/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public string waveName;

    public float startDelay;       // Wave開始前の待ち時間（演出用）
    public EnemyGroup[] enemyGroups;
    public EnemyStatMultiplier enemyStatMultiplier;

    // Wave クリア条件
    public WaveClearType clearType;

    // SurviveTime 用
    public float surviveDuration = 0f;
    public bool isBossWave;

    // Wave終了後のChestスポーン
    [Header("Reward Chest")]
    public bool spawnHealChestOnClear = false;
    public bool spawnAttackChestOnClear = false;
    public bool spawnDefenseChestOnClear = false;


}


