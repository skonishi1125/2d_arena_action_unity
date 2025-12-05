using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;

    private bool isSpawning;
    private Coroutine spawnLoopCo;

    public void BeginSpawn()
    {
        if (isSpawning)
            return;

        isSpawning = true;
        spawnLoopCo = StartCoroutine(SpawnLoop());
    }

    public void StopSpawn()
    {
        isSpawning = false;
        if (spawnLoopCo != null)
        {
            StopCoroutine(spawnLoopCo);
            spawnLoopCo = null;
        }
    }


    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
            return;

        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, point.position, Quaternion.identity);
    }

}
