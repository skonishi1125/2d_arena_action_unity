using UnityEngine;

public class EnemySpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public void Spawn(GameObject enemyPrefab)
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
            return;

        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, point.position, Quaternion.identity);
    }
}
