using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Color iconColor = Color.red;
    [SerializeField] private float iconHeight = 0.5f;
    [SerializeField] private Vector3 iconScale = new Vector3(0.3f, 0.3f, 0.3f);

    // Shader 참조 추가 (빌드에 포함시키기 위해)
    [SerializeField] private Shader iconShader;

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

            // Shader 설정 개선 (빌드 호환성)
            if (iconShader != null)
            {
                renderer.material = new Material(iconShader);
            }
            else
            {
                // Sprites/Default 또는 UI/Default 사용
                Shader shader = Shader.Find("UI/Default");
                if (shader == null)
                {
                    shader = Shader.Find("Sprites/Default");
                }
                if (shader == null)
                {
                    shader = Shader.Find("Unlit/Color");
                }

                if (shader != null)
                {
                    renderer.material = new Material(shader);
                }
                else
                {
                    Debug.LogError("사용 가능한 Shader를 찾을 수 없습니다!");
                }
            }

            renderer.material.color = iconColor;

            // 양면 렌더링 설정 (회전 문제 방지)
            renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }

        iconInstance.transform.position = transform.position + Vector3.up * iconHeight;
        iconInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
        iconInstance.transform.localScale = iconScale;

        // 레이어 설정
        int minimapLayer = LayerMask.NameToLayer("Minimap");
        if (minimapLayer == -1)
        {
            Debug.LogWarning("Minimap 레이어가 없습니다. 기본 레이어를 사용합니다.");
            minimapLayer = 0;
        }
        iconInstance.layer = minimapLayer;

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