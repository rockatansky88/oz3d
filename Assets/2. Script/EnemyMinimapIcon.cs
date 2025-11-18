using UnityEngine;

/// <summary>
/// 적 미니맵 아이콘 - 빨간색 원형 (방향 표시 없음)
/// </summary>
public class EnemyMinimapIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private Color iconColor = Color.yellow;
    [SerializeField] private float iconHeight = 15f;
    [SerializeField] private Vector3 iconScale = new Vector3(1.2f, 1.2f, 1.2f);

    // Shader 참조 추가 (빌드 호환성)
    [SerializeField] private Shader iconShader;

    private GameObject iconInstance;

    void Start()
    {
        CreateEnemyIcon();
    }

    void LateUpdate()
    {
        if (iconInstance != null)
        {
            // Enemy 위치만 추적 (회전 없음)
            iconInstance.transform.position = transform.position + Vector3.up * iconHeight;
        }
    }

    private void CreateEnemyIcon()
    {
        GameObject parentObj = GameObject.Find("MinimapIcons");
        if (parentObj == null)
        {
            parentObj = new GameObject("MinimapIcons");
        }

        iconInstance = GameObject.CreatePrimitive(PrimitiveType.Quad);
        iconInstance.transform.SetParent(parentObj.transform);
        iconInstance.name = "Enemy_MinimapIcon";

        // Collider 제거 (충돌 방지)
        Collider col = iconInstance.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        // 원형 텍스처 생성
        Texture2D circleTexture = CreateCircleTexture();

        Renderer renderer = iconInstance.GetComponent<Renderer>();

        // Shader 설정 개선 (빌드 호환성)
        if (iconShader != null)
        {
            renderer.material = new Material(iconShader);
        }
        else
        {
            // 여러 Shader 시도
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
                return;
            }
        }

        renderer.material.mainTexture = circleTexture;
        renderer.material.color = iconColor;

        // 양면 렌더링 설정
        renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        // 고정 회전 (미니맵 카메라를 향함)
        iconInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
        iconInstance.transform.localScale = iconScale;

        // 레이어 설정
        int minimapLayer = LayerMask.NameToLayer("Minimap");
        if (minimapLayer == -1)
        {
            minimapLayer = 0;
        }
        iconInstance.layer = minimapLayer;
    }

    // 원형 텍스처 생성 (적 아이콘)
    private Texture2D CreateCircleTexture()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // 모든 픽셀 투명하게 초기화
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        // 원형 그리기
        int centerX = size / 2;
        int centerY = size / 2;
        int radius = size / 2 - 2; // 약간 여백

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // 중심으로부터의 거리 계산
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                // 원 안쪽이면 흰색으로 채우기
                if (distance <= radius)
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
