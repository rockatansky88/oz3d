using UnityEngine;

// 미니맵에 표시될 아이콘을 자동으로 생성하고 관리 (색상 설정 가능)
public class MinimapIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Color iconColor = Color.red;  // 아이템은 빨간색
    [SerializeField] private float iconHeight = 0.5f;
    [SerializeField] private Vector3 iconScale = new Vector3(0.3f, 0.3f, 0.3f);

    private GameObject iconInstance;
    private Transform minimapIconsParent;

    void Start()
    {
        CreateMinimapIcon();
    }

    void LateUpdate()
    {
        if (iconInstance != null)
        {
            iconInstance.transform.position = transform.position + Vector3.up * iconHeight;
        }
    }

    //미니맵 아이콘 생성
    private void CreateMinimapIcon()
    {
        GameObject parentObj = GameObject.Find("MinimapIcons"); //미니맵 아이콘들을 모아둘 부모 오브젝트 찾기
        if (parentObj == null)
        {
            parentObj = new GameObject("MinimapIcons");
        }
        minimapIconsParent = parentObj.transform;

        if (iconPrefab != null)
        {
            iconInstance = Instantiate(iconPrefab, minimapIconsParent); //미니맵 아이콘 프리팹 인스턴스화
        }
        else
        {
            iconInstance = GameObject.CreatePrimitive(PrimitiveType.Quad); //프리팹이 없으면 기본 쿼드 생성
            iconInstance.transform.SetParent(minimapIconsParent);  // 부모 설정

            Destroy(iconInstance.GetComponent<Collider>()); // 충돌체 제거

            Renderer renderer = iconInstance.GetComponent<Renderer>();  // 렌더러 가져오기
            renderer.material = new Material(Shader.Find("Unlit/Color")); // 언리트 컬러 셰이더 사용
            renderer.material.color = iconColor; // 아이콘 색상 설정
        }

        iconInstance.transform.position = transform.position + Vector3.up * iconHeight; // 아이콘 위치 설정
        iconInstance.transform.rotation = Quaternion.Euler(90, 0, 0);  // 아이콘 회전 설정 (위에서 내려다보는 방향)
        iconInstance.transform.localScale = iconScale;  // 아이콘 크기 설정

        iconInstance.layer = LayerMask.NameToLayer("Minimap"); //미니맵 레이어 설정
        if (iconInstance.layer == -1)
        {
            Debug.LogWarning("Minimap 레이어가 없습니다. Project Settings > Tags and Layers에서 'Minimap' 레이어를 추가하세요.");
            iconInstance.layer = 0;
        }

        iconInstance.name = gameObject.name + "_MinimapIcon";
    }

    public void RemoveIcon()
    {
        if (iconInstance != null)
        {
            Destroy(iconInstance);
        }
    }

    void OnDestroy()
    {
        if (iconInstance != null)
        {
            Destroy(iconInstance);
        }
    }
}