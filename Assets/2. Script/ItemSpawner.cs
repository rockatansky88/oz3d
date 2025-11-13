using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject itemPrefab;     // 아이템 프리팹
    [SerializeField] private Transform spawnPlane;      // 스폰 영역 (Plane)
    [SerializeField] private int itemCount = 10;        // 스폰할 아이템 개수
    [SerializeField] private float spawnHeight = 1f;    // 스폰 높이

    [Header("Spawn Area (Plane 크기 기반)")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f); // Plane 크기

    private ObjectPool objectPool;

    void Start()
    {
        // ObjectPool 생성
        GameObject poolObj = new GameObject("ItemPool");
        poolObj.transform.SetParent(transform);
        objectPool = poolObj.AddComponent<ObjectPool>();

        objectPool.Initialize(itemPrefab, itemCount);

        // 아이템 스폰
        Invoke(nameof(SpawnItems), 0.1f);
    }

    //랜덤하게 아이템을 스폰 
    private void SpawnItems()
    {
        Vector3 planeCenter = spawnPlane != null ? spawnPlane.position : Vector3.zero;

        for (int i = 0; i < itemCount; i++)
        {
            // 랜덤 위치 계산 (Plane 내부)
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

            Vector3 spawnPosition = new Vector3(
                planeCenter.x + randomX,
                planeCenter.y + spawnHeight,
                planeCenter.z + randomZ
            );

            // 오브젝트 풀에서 아이템 가져오기
            GameObject item = objectPool.GetObject();
            item.transform.position = spawnPosition;
            item.transform.rotation = Quaternion.identity;

            Debug.Log($"아이템 {i + 1} 스폰: {spawnPosition}");
        }
    }

    // Scene View에서 스폰 영역 시각화
    void OnDrawGizmosSelected()
    {
        if (spawnPlane == null) return;

        Gizmos.color = Color.green;
        Vector3 center = spawnPlane.position + Vector3.up * spawnHeight;
        Gizmos.DrawWireCube(center, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}