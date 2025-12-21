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

    // 救済Chestの有無
    [Header("Reward Chest (on clear)")]
    public bool spawnRewardChestOnClear = false;
    //[Range(0f, 1f)]
    //public float rewardChestChance = 1f; // 確率

}

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int spawnCount;         // 何体出すか
    public float spawnInterval;    // 何秒おきに出すか
    public EnemyRole enemyRole;    // Wanderer or Raider
}
