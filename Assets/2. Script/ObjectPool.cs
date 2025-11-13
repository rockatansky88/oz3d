using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Transform poolParent;

    // 초기화 메서드 추가
    public void Initialize(GameObject prefabToUse, int poolSize)
    {
        prefab = prefabToUse;
        initialPoolSize = poolSize;

        // 풀 부모 오브젝트 생성
        poolParent = new GameObject($"{prefab.name}_Pool").transform;
        poolParent.SetParent(transform);

        // 초기 오브젝트 생성
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewObject();
        }
    }

    void Start()
    {
        // Initialize가 호출되지 않았을 때만 실행
        if (prefab != null && poolParent == null)
        {
            Initialize(prefab, initialPoolSize);
        }
    }

    // 새로운 오브젝트 생성
    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, poolParent);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    //풀에서 오브젝트 가져오기
    public GameObject GetObject()
    {
        // 풀이 비어있으면 새로 생성
        if (pool.Count == 0)
        {
            CreateNewObject();
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    // 오브젝트 반환
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        pool.Enqueue(obj);
    }
}