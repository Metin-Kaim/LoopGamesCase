using Assets.CoreGame.Scripts.Signals;
using UnityEngine;

public class ErasableGround : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int textureSize = 512;
    [SerializeField] private float eraseRadius = 0.5f;
    [SerializeField][Range(0f, 1f)] private float eraseStrength = 0.3f;
    [SerializeField] private bool autoEraseOnUpdate = false;
    [SerializeField] private Transform player;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Texture2D groundTexture;

    [Header("Area Settings")]
    [SerializeField] private Vector2 groundSize = new Vector2(10f, 10f); // Zeminin gerçek boyutu (world space)
    [SerializeField] private bool autoDetectSize = true; // Otomatik boyut algýla

    private Texture2D maskTexture;
    private Color[] clearColors;
    private bool isDirty = false;
    private Bounds groundBounds;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = TextureToSprite(groundTexture);

        Vector2 gameBoundary = GameSignals.Instance.GetGameArea.Invoke();
        float scaleX = gameBoundary.x * 96 / groundTexture.width;
        float scaleY = gameBoundary.y * 96 / groundTexture.height;
        transform.localScale = new Vector3(scaleX, scaleY, 1);

        InitializeMask();
    }
    public Sprite TextureToSprite(
       Texture2D texture,
       float pixelsPerUnit = 100f,
       Vector2? pivot = null)
    {
        if (texture == null)
        {
            Debug.LogError("Texture null, Sprite oluþturulamaz.");
            return null;
        }

        Vector2 finalPivot = pivot ?? new Vector2(0.5f, 0.5f);

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            finalPivot,
            pixelsPerUnit
        );
    }

    void LateUpdate()
    {
        if (isDirty)
        {
            maskTexture.Apply();
            isDirty = false;
        }
    }

    private void InitializeMask()
    {
        // Ground bounds'u hesapla
        CalculateGroundBounds();

        // Mask texture oluþtur
        maskTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.wrapMode = TextureWrapMode.Clamp;

        // Baþlangýçta tamamen beyaz (görünür)
        clearColors = new Color[textureSize * textureSize];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        maskTexture.SetPixels(clearColors);
        maskTexture.Apply();

        // Material'e ata
        groundMaterial.SetTexture("_MaskTex", maskTexture);
    }

    private void CalculateGroundBounds()
    {
        if (autoDetectSize)
        {
            // SpriteRenderer'dan otomatik hesapla
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                groundBounds = spriteRenderer.bounds;
            }
            // Veya MeshRenderer'dan
            else
            {
                Renderer rend = GetComponent<Renderer>();
                if (rend != null)
                {
                    groundBounds = rend.bounds;
                }
                else
                {
                    // Fallback: Transform scale'den hesapla (2D için X ve Y)
                    Vector3 scale = transform.lossyScale;
                    groundBounds = new Bounds(transform.position, new Vector3(scale.x, scale.y, 0.1f));
                }
            }
        }
        else
        {
            // Manuel boyut kullan (2D için X ve Y)
            groundBounds = new Bounds(
                transform.position,
                new Vector3(groundSize.x, groundSize.y, 0.1f)
            );
        }
    }

    public void EraseAtWorldPosition(Vector3 worldPos)
    {
        // World pozisyonunu UV'ye çevir (bounds-based)
        Vector2 uv = WorldToUV(worldPos);

        // UV bounds dýþýndaysa çýk
        if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            return;

        // UV'yi texture koordinatýna çevir
        int centerX = Mathf.RoundToInt(uv.x * textureSize);
        int centerY = Mathf.RoundToInt(uv.y * textureSize);

        // Radius'u texture space'e çevir (bounds boyutuna göre)
        float avgGroundSize = (groundBounds.size.x + groundBounds.size.y) * 0.5f;
        int pixelRadius = Mathf.RoundToInt((eraseRadius / avgGroundSize) * textureSize);

        // Dairesel gradient silme (brush efekti)
        for (int x = -pixelRadius; x <= pixelRadius; x++)
        {
            for (int y = -pixelRadius; y <= pixelRadius; y++)
            {
                int pixelX = centerX + x;
                int pixelY = centerY + y;

                // Bounds check
                if (pixelX >= 0 && pixelX < textureSize && pixelY >= 0 && pixelY < textureSize)
                {
                    // Merkezden uzaklýk
                    float distance = Mathf.Sqrt(x * x + y * y);

                    // Daire içindeyse
                    if (distance <= pixelRadius)
                    {
                        // Gradient hesapla (merkez 1, kenar 0)
                        float falloff = 1f - (distance / pixelRadius);
                        falloff = Mathf.SmoothStep(0f, 1f, falloff);

                        // Mevcut alpha'yý al
                        Color currentColor = maskTexture.GetPixel(pixelX, pixelY);

                        // Yavaþça sil (brush efekti)
                        float eraseAmount = falloff * eraseStrength;
                        currentColor.a = Mathf.Max(0f, currentColor.a - eraseAmount);

                        maskTexture.SetPixel(pixelX, pixelY, currentColor);
                        isDirty = true;
                    }
                }
            }
        }
    }

    public void EraseAtUV(Vector2 uv)
    {
        Vector3 worldPos = UVToWorld(uv);
        EraseAtWorldPosition(worldPos);
    }

    public void EraseAreaCompletely(Vector3 worldPos, float radius)
    {
        float originalRadius = eraseRadius;
        float originalStrength = eraseStrength;

        eraseRadius = radius;
        eraseStrength = 1f;

        for (int i = 0; i < 5; i++)
        {
            EraseAtWorldPosition(worldPos);
        }

        eraseRadius = originalRadius;
        eraseStrength = originalStrength;

        if (isDirty)
        {
            maskTexture.Apply();
            isDirty = false;
        }
    }

    public void ResetMask()
    {
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        maskTexture.SetPixels(clearColors);
        maskTexture.Apply();
        isDirty = false;
    }

    public float GetErasePercentage()
    {
        Color[] pixels = maskTexture.GetPixels();
        float totalAlpha = 0f;

        for (int i = 0; i < pixels.Length; i++)
        {
            totalAlpha += pixels[i].a;
        }

        return 1f - (totalAlpha / pixels.Length);
    }

    private Vector2 WorldToUV(Vector3 worldPos)
    {
        // World pozisyonunu bounds içindeki relative pozisyona çevir (2D: X ve Y)
        Vector3 min = groundBounds.min;
        Vector3 max = groundBounds.max;

        // X ve Y ekseni için normalize et
        float u = Mathf.InverseLerp(min.x, max.x, worldPos.x);
        float v = Mathf.InverseLerp(min.y, max.y, worldPos.y); // 2D için Y ekseni

        return new Vector2(u, v);
    }

    private Vector3 UVToWorld(Vector2 uv)
    {
        Vector3 min = groundBounds.min;
        Vector3 max = groundBounds.max;

        float worldX = Mathf.Lerp(min.x, max.x, uv.x);
        float worldY = Mathf.Lerp(min.y, max.y, uv.y); // 2D için Y ekseni

        return new Vector3(worldX, worldY, transform.position.z); // Z'yi koru
    }

    // Runtime'da ground boyutu deðiþirse bunu çaðýr
    public void RecalculateBounds()
    {
        CalculateGroundBounds();
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Ground bounds'u çiz (2D için XY düzlemi)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundBounds.center, groundBounds.size);

            // Erase radius'u çiz
            if (autoEraseOnUpdate && player != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(player.position, eraseRadius);
            }
        }
        else
        {
            // Editor'da preview için
            if (autoDetectSize)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(spriteRenderer.bounds.center, spriteRenderer.bounds.size);
                }
                else
                {
                    Renderer rend = GetComponent<Renderer>();
                    if (rend != null)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(rend.bounds.center, rend.bounds.size);
                    }
                }
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(transform.position, new Vector3(groundSize.x, groundSize.y, 0.1f));
            }
        }
    }
}