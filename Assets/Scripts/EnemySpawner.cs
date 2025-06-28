using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // các điểm spawn ngẫu nhiên

    public void RespawnAfterDelay(float delay)
    {
        StartCoroutine(RespawnCoroutine(delay));
    }

    private IEnumerator RespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Chưa gán spawnPoints cho EnemySpawner!");
            yield break;
        }

        // Chọn vị trí spawn ngẫu nhiên
        int index = Random.Range(0, spawnPoints.Length);
        Vector3 randomSpawnPosition = spawnPoints[index].position;

        GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPosition, Quaternion.identity);
        EnemyController ec = newEnemy.GetComponent<EnemyController>();
        if (ec != null)
        {
            ec.SetSpawner(this);
        }
    }
}

