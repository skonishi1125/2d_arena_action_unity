using UnityEngine;

public class EnemySpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public void Spawn(GameObject enemyPrefab)
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
            return;

        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var go = Instantiate(enemyPrefab, point.position, Quaternion.identity);

        // どちらを向いて生まれるか、ランダムに決定
        var enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            int dir = Random.value < 0.5f ? -1 : 1;
            enemy.InitializeFacing(dir);
        }

    }
}
