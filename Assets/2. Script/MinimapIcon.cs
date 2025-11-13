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

    private void CreateMinimapIcon()
    {
        GameObject parentObj = GameObject.Find("MinimapIcons");
        if (parentObj == null)
        {
            parentObj = new GameObject("MinimapIcons");
        }
        minimapIconsParent = parentObj.transform;

        if (iconPrefab != null)
        {
            iconInstance = Instantiate(iconPrefab, minimapIconsParent);
        }
        else
        {
            iconInstance = GameObject.CreatePrimitive(PrimitiveType.Quad);
            iconInstance.transform.SetParent(minimapIconsParent);

            Destroy(iconInstance.GetComponent<Collider>());

            Renderer renderer = iconInstance.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Unlit/Color"));
            renderer.material.color = iconColor;
        }

        iconInstance.transform.position = transform.position + Vector3.up * iconHeight;
        iconInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
        iconInstance.transform.localScale = iconScale;

        iconInstance.layer = LayerMask.NameToLayer("Minimap");
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