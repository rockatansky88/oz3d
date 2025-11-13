using UnityEngine;

/// <summary>
/// 플레이어 미니맵 아이콘 - 초록색 + 진행 방향 화살표
/// </summary>
public class PlayerMinimapIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private Color iconColor = Color.green;
    [SerializeField] private float iconHeight = 1f;
    [SerializeField] private Vector3 iconScale = new Vector3(0.5f, 0.5f, 0.5f);

    // Shader 참조 추가 (빌드 호환성)
    [SerializeField] private Shader iconShader;

    private GameObject iconInstance;

    void Start()
    {
        CreatePlayerIcon();
    }

    void LateUpdate()
    {
        if (iconInstance != null)
        {
            iconInstance.transform.position = transform.position + Vector3.up * iconHeight;
            iconInstance.transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
        }
    }

    private void CreatePlayerIcon()
    {
        GameObject parentObj = GameObject.Find("MinimapIcons");
        if (parentObj == null)
        {
            parentObj = new GameObject("MinimapIcons");
        }

        iconInstance = GameObject.CreatePrimitive(PrimitiveType.Quad);
        iconInstance.transform.SetParent(parentObj.transform);
        iconInstance.name = "Player_MinimapIcon";

        // Collider 제거 (충돌 방지)
        Collider col = iconInstance.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        Texture2D arrowTexture = CreateArrowTexture();

        Renderer renderer = iconInstance.GetComponent<Renderer>();

        // Shader 설정 개선 (빌드 호환성)
        if (iconShader != null)
        {
            renderer.material = new Material(iconShader);
        }
        else
        {
            // 여러 Shader 
            Shader shader = Shader.Find("UI/Default");
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Transparent");
            }

            if (shader != null)
            {
                renderer.material = new Material(shader);
            }
            else
            {
                Debug.LogError("PlayerMinimapIcon: 사용 가능한 Shader를 찾을 수 없습니다!");
                return;
            }
        }

        renderer.material.mainTexture = arrowTexture;
        renderer.material.color = iconColor;

        // 양면 렌더링 설정
        renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        iconInstance.transform.localScale = iconScale;

        // 레이어 설정
        int minimapLayer = LayerMask.NameToLayer("Minimap");
        if (minimapLayer == -1)
        {
            Debug.LogWarning("Minimap 레이어가 없습니다. 기본 레이어를 사용합니다.");
            minimapLayer = 0;
        }
        iconInstance.layer = minimapLayer;
    }

    // 화살표 텍스처 생성 (플레이어 진행 방향 표시)
    private Texture2D CreateArrowTexture()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // 모든 픽셀 투명하게 초기화
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        // 삼각형 화살표 그리기 (위쪽이 뾰족함)
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int centerX = size / 2;
                int width = (size - y) / 2;

                // 삼각형 영역에만 색상 채우기
                if (x >= centerX - width && x <= centerX + width && y > size / 4)
                {
                    pixels[y * size + x] = Color.white;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    void OnDestroy()
    {
        if (iconInstance != null)
        {
            Destroy(iconInstance);
        }
    }
}