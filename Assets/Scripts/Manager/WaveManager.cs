using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private EnemySpawnPoints enemySpawnPoints;
    [SerializeField] private ChestDropPoints rewardChestDropPoints;
    [SerializeField] private GameObject defaultRewardChestPrefab;

    private int currentWaveIndex = -1; // なぜかハイライトされていないが使ってる
    private int aliveEnemyCount;
    private Coroutine stageRoutine;
    private bool isRunning;
    private bool isBossDefeated = false;

    // ボスウェーブであることの通知
    public event Action<WaveConfig> OnBossWaveStarted;

    // Stageクリア通知 GameManagerなどで購読する
    public event Action OnStageCleared;

    private void OnEnable()
    {
        EnemyHealth.OnAnyEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        EnemyHealth.OnAnyEnemyDied -= HandleEnemyDied;
    }

    public void BeginStage()
    {
        if (isRunning)
            return;

        isRunning = true;
        stageRoutine = StartCoroutine(RunStage());
    }


    public void StopStage()
    {
        if (!isRunning)
            return;

        isRunning = false;
        if (stageRoutine != null)
        {
            StopCoroutine(stageRoutine);
            stageRoutine = null;
        }
    }

    // Stage全体の進行担当
    private IEnumerator RunStage()
    {
        // stageConfigに持たせたWave配列の分だけ回す
        for (int i = 0; i < stageConfig.waves.Length; i++)
        {
            if (!isRunning)
                yield break;

            currentWaveIndex = i;
            var wave = stageConfig.waves[i];
            yield return StartCoroutine(RunWave(wave));
        }

        // ステージ終了( = 全てのWaveの呼び出し完了)
        isRunning = false;
        OnStageCleared?.Invoke();
    }

    // Wave単体の進行を担当
    private IEnumerator RunWave(WaveConfig wave)
    {
        // ボスWave時 フラグをリセットしてWARNING等の表示
        if (wave.isBossWave)
        {
            isBossDefeated = false;
            OnBossWaveStarted?.Invoke(wave);
        }

        // Wave開始前待機時間
        yield return new WaitForSeconds(wave.startDelay);

        aliveEnemyCount = 0;

        var mul = wave.enemyStatMultiplier;

        // Waveに持たせている敵情報の数だけ回す
        foreach (var group in wave.enemyGroups)
        {
            if (!isRunning)
                yield break;

            yield return StartCoroutine(SpawnGroup(group, mul));
        }

        switch (wave.clearType)
        {
            case WaveClearType.SurviveTime:
                // 一定時間生き延びればOK
                float t = wave.surviveDuration;
                while (t > 0f && isRunning)
                {
                    t -= Time.deltaTime;
                    yield return null;
                }
                break;

            case WaveClearType.KillAll:
                // 全滅するまで待つ（BossWave もこれ）
                while (aliveEnemyCount > 0 && isRunning)
                    yield return null;
                break;

            case WaveClearType.KillBoss:
                while (! isBossDefeated && isRunning)
                    yield return null;
                break;
        }

        // Wave終了後のチェストスポーン判定
        TrySpawnRewardChestOnWaveClear(wave);

    }

    // Waveに持たせた、敵配列自体のスポーン処理
    private IEnumerator SpawnGroup(EnemyGroup group, EnemyStatMultiplier mul)
    {
        for (int i = 0; i < group.spawnCount; i++)
        {
            if (!isRunning)
                yield break;

            var enemyPrefab = group.enemyPrefab;
            if (!LogHelper.AssertNotNull(enemyPrefab, nameof(enemyPrefab), this))
                yield break;

            enemySpawnPoints.Spawn(group.enemyPrefab, group.enemyRole, mul); // multiplier追加？
            aliveEnemyCount++;

            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    // 敵のカウントを減らす処理
    // 現状まだ使っていないが、使う場合はEnemyHealthイベントに購読させる。
    public void HandleEnemyDied(EnemyHealth enemyHealth)
    {
        if (!isRunning)
            return;

        aliveEnemyCount--;

        if (enemyHealth.IsBoss)
        {
            isBossDefeated = true;
            OnStageCleared?.Invoke();
        }

    }

    // Wave終了時の救済チェスト出現処理
    private void TrySpawnRewardChestOnWaveClear(WaveConfig wave)
    {
        if (!isRunning)
            return;

        if (wave == null)
            return;

        if (!wave.spawnRewardChestOnClear)
            return;

        // チェストを出す確率をつけるなら、こちらで管理する
        //if (wave.rewardChestChance < 1f && UnityEngine.Random.value > wave.rewardChestChance)
        //    return;

        var prefab = defaultRewardChestPrefab;

        if (prefab == null)
            return;

        if (rewardChestDropPoints == null)
            return;

        var p = rewardChestDropPoints.GetRandomPoint();
        if (p == null)
            return;

        Instantiate(prefab, p.position, Quaternion.identity);
    }

}
