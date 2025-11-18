using UnityEngine;

// 랜덤하게 적을 생성
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPlane;
    [SerializeField] private int enemyCount = 2;
    [SerializeField] private float spawnHeight = 1f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(20f, 20f);

    private ObjectPool objectPool;

    void Start()
    {
        // ObjectPool 생성
        GameObject poolObj = new GameObject("EnemyPool");
        poolObj.transform.SetParent(transform);
        objectPool = poolObj.AddComponent<ObjectPool>();

        objectPool.Initialize(enemyPrefab, enemyCount);

        // 적 생성
        Invoke(nameof(SpawnEnemies), 0.1f);
    }

    // 랜덤하게 적 생성
    private void SpawnEnemies()
    {
        Vector3 planeCenter = spawnPlane != null ? spawnPlane.position : Vector3.zero;

        for (int i = 0; i < enemyCount; i++)
        {
            // 랜덤 위치 계산 (Plane 범위 내)
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

            Vector3 spawnPosition = new Vector3(
                planeCenter.x + randomX,
                planeCenter.y + spawnHeight,
                planeCenter.z + randomZ
            );

            // 오브젝트 풀에서 적 가져오기
            GameObject enemy = objectPool.GetObject();
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            Debug.Log($"적 {i + 1} 생성: {spawnPosition}");
        }
    }

    // Scene View에서 생성 영역 시각화
    void OnDrawGizmosSelected()
    {
        if (spawnPlane == null) return;

        Gizmos.color = Color.red;
        Vector3 center = spawnPlane.position + Vector3.up * spawnHeight;
        Gizmos.DrawWireCube(center, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}