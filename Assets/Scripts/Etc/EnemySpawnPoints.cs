using UnityEngine;

public class EnemySpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public void Spawn(GameObject enemyPrefab, EnemyRole role, EnemyStatMultiplier mul)
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
            return;

        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var go = Instantiate(enemyPrefab, point.position, Quaternion.identity);

        // Wave倍率設定
        // maxHpだけ上がっているのでcurrentHpも合わせる。
        // 経験値も指定の倍数だけ再設定。
        ApplyMultiplier(go, mul);
        var health = go.GetComponent<EnemyHealth>();
        if (health != null)
            health.ResetHpToMax();
        var reward = go.GetComponent<EnemyReward>();
        if (reward != null)
            reward.ApplyExpMultiplier(mul.exp);

        // どちらを向いて生まれるか、ランダムに決定
        var enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            int dir = Random.value < 0.5f ? -1 : 1;
            enemy.role = role;
            enemy.InitializeFacing(dir);
        }

    }
    private static void ApplyMultiplier(GameObject go, EnemyStatMultiplier mul)
    {
        var status = go.GetComponent<EntityStatus>();
        if (status == null) return;

        // Status.AddMultiplier は「multiplierに加算」なので (倍率 - 1) を渡す
        // 1と指定したなら1倍、 2なら2-1 で1倍ぶん加算
        status.maxHp.AddMultiplier(mul.hp - 1f);
        status.attack.AddMultiplier(mul.atk - 1f);
        status.defense.AddMultiplier(mul.def - 1f);
        status.evasion.AddMultiplier(mul.eva - 1f);
        status.critical.AddMultiplier(mul.crit - 1f);
    }
}
