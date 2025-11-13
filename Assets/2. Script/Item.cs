using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private float rotationSpeed = 50f; // 회전 속도
    [SerializeField] private AudioClip collectSound;    // 수집 사운드
    [SerializeField] private GameObject collectEffect;  // 수집 이펙트 프리팹

    private MinimapIcon minimapIcon;                    // 미니맵 아이콘 참조

    void Awake()
    {
        // Collider를 Trigger로 설정
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // MinimapIcon 컴포넌트 참조
        minimapIcon = GetComponent<MinimapIcon>();
    }

    void Update()
    {
        // 아이템 회전 애니메이션
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌 시
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    //아이템 수집 처리
    private void CollectItem()
    {
        // 1. GameManager에 수집 알림
        GameManager.instance.CollectItem();

        // 2. 미니맵 아이콘 제거
        if (minimapIcon != null)
        {
            minimapIcon.RemoveIcon();
        }

        // 3. 사운드 재생
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // 4. 이펙트 생성
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 5. 오브젝트 비활성화 (풀로 반환하거나 파괴)
        gameObject.SetActive(false);

        Debug.Log($"{gameObject.name} 아이템 수집!");
    }
}