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

        // ⭐ Collider 제거 (충돌 방지)
        Collider col = iconInstance.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        Texture2D arrowTexture = CreateArrowTexture();

        Renderer renderer = iconInstance.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Unlit/Transparent"));
        renderer.material.mainTexture = arrowTexture;
        renderer.material.color = iconColor;

        iconInstance.transform.localScale = iconScale;
        iconInstance.layer = LayerMask.NameToLayer("Minimap");
        if (iconInstance.layer == -1)
        {
            iconInstance.layer = 0;
        }
    }

    private Texture2D CreateArrowTexture()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int centerX = size / 2;
                int width = (size - y) / 2;

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