using Assets.CoreGame.Scripts.Signals;
using UnityEngine;

public class ErasableGround : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int textureSize = 512;
    [SerializeField] private float eraseRadius = 0.5f;
    [SerializeField][Range(0f, 1f)] private float eraseStrength = 0.3f;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Texture2D groundTexture;

    [Header("Area Settings")]
    [SerializeField] private Vector2 groundSize = new Vector2(10f, 10f);
    [SerializeField] private bool autoDetectSize = true;
    [SerializeField] private Vector2 textureTiling;

    [Header("Optimization")]
    [SerializeField] private int maxErasesPerFrame = 5; // Frame baþýna max silme sayýsý

    private Texture2D maskTexture;
    private Color32[] maskColors; // Color32 daha hýzlý (byte-based)
    private Bounds groundBounds;
    private int pendingEraseCount = 0;

    // Cache
    private SpriteRenderer spriteRenderer;
    private float textureSizeRecip; // 1/textureSize - division yerine multiplication
    private float avgGroundSizeRecip;

    private void OnEnable()
    {
        ScratchSignals.Instance.OnScratchAtPosition += EraseAtWorldPosition;
    }

    private void OnDisable()
    {
        ScratchSignals.Instance.OnScratchAtPosition -= EraseAtWorldPosition;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = TextureToSprite(groundTexture);

        if (textureTiling.x != 0 && textureTiling.y != 0)
            groundMaterial.SetVector("_TextureTiling", textureTiling);

        Vector2 gameBoundary = GameSignals.Instance.GetGameArea.Invoke();
        float tileSize = GameSignals.Instance.GetTileSize.Invoke();

        float scaleX = gameBoundary.x * (tileSize * 100) / groundTexture.width;
        float scaleY = gameBoundary.y * (tileSize * 100) / groundTexture.height;
        transform.localScale = new Vector3(scaleX, scaleY, 1);

        InitializeMask();
    }

    public Sprite TextureToSprite(Texture2D texture, float pixelsPerUnit = 100f, Vector2? pivot = null)
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
        if (pendingEraseCount > 0)
        {
            maskTexture.SetPixels32(maskColors); // Tek seferde tüm array'i yükle
            maskTexture.Apply(false); // mipmap güncelleme false - daha hýzlý
            pendingEraseCount = 0;
        }
    }

    private void InitializeMask()
    {
        CalculateGroundBounds();

        // Cache reciprocals
        textureSizeRecip = 1f / textureSize;
        float avgGroundSize = (groundBounds.size.x + groundBounds.size.y) * 0.5f;
        avgGroundSizeRecip = 1f / avgGroundSize;

        maskTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.wrapMode = TextureWrapMode.Clamp;

        // Color32 kullan (4 byte, Color'dan daha hýzlý)
        int totalPixels = textureSize * textureSize;
        maskColors = new Color32[totalPixels];

        // Baþlangýçta tamamen opak beyaz
        for (int i = 0; i < totalPixels; i++)
        {
            maskColors[i] = new Color32(255, 255, 255, 255);
        }

        maskTexture.SetPixels32(maskColors);
        maskTexture.Apply(false);

        groundMaterial.SetTexture("_MaskTex", maskTexture);
    }

    private void CalculateGroundBounds()
    {
        if (autoDetectSize)
        {
            if (spriteRenderer != null)
            {
                groundBounds = spriteRenderer.bounds;
            }
            else
            {
                Renderer rend = GetComponent<Renderer>();
                if (rend != null)
                {
                    groundBounds = rend.bounds;
                }
                else
                {
                    Vector3 scale = transform.lossyScale;
                    groundBounds = new Bounds(transform.position, new Vector3(scale.x, scale.y, 0.1f));
                }
            }
        }
        else
        {
            groundBounds = new Bounds(
                transform.position,
                new Vector3(groundSize.x, groundSize.y, 0.1f)
            );
        }
    }

    public void EraseAtWorldPosition(Vector2 worldPos)
    {
        // Frame limiti - performans korumasý
        if (pendingEraseCount >= maxErasesPerFrame)
            return;

        Vector2 uv = WorldToUV(worldPos);

        // Early exit - bounds check
        if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            return;

        // Multiplication ile texture koordinatý (division yerine)
        int centerX = Mathf.RoundToInt(uv.x * textureSize);
        int centerY = Mathf.RoundToInt(uv.y * textureSize);

        // Pixel radius hesapla (cached reciprocal kullan)
        int pixelRadius = Mathf.RoundToInt(eraseRadius * avgGroundSizeRecip * textureSize);

        // Squared radius - sqrt hesabýný önlemek için
        int pixelRadiusSq = pixelRadius * pixelRadius;

        // Min/max bounds hesapla - döngü optimizasyonu
        int minX = Mathf.Max(0, centerX - pixelRadius);
        int maxX = Mathf.Min(textureSize - 1, centerX + pixelRadius);
        int minY = Mathf.Max(0, centerY - pixelRadius);
        int maxY = Mathf.Min(textureSize - 1, centerY + pixelRadius);

        // Dairesel silme - optimized loop
        for (int y = minY; y <= maxY; y++)
        {
            int dy = y - centerY;
            int dySq = dy * dy;

            for (int x = minX; x <= maxX; x++)
            {
                int dx = x - centerX;
                int distanceSq = dx * dx + dySq; // Squared distance

                // Sqrt yerine squared karþýlaþtýrma
                if (distanceSq <= pixelRadiusSq)
                {
                    int index = y * textureSize + x;

                    // Gradient hesapla
                    float distance = Mathf.Sqrt(distanceSq); // Sadece gerektiðinde sqrt
                    float falloff = 1f - (distance / pixelRadius);
                    falloff = Mathf.SmoothStep(0f, 1f, falloff);

                    // Alpha reduction (byte hesaplama)
                    byte currentAlpha = maskColors[index].a;
                    float eraseAmount = falloff * eraseStrength;
                    byte newAlpha = (byte)Mathf.Max(0, currentAlpha - (byte)(eraseAmount * 255));

                    maskColors[index].a = newAlpha;
                }
            }
        }

        pendingEraseCount++;
    }

    private Vector2 WorldToUV(Vector3 worldPos)
    {
        Vector3 min = groundBounds.min;
        Vector3 max = groundBounds.max;

        // InverseLerp inline - daha hýzlý
        float u = (worldPos.x - min.x) / (max.x - min.x);
        float v = (worldPos.y - min.y) / (max.y - min.y);

        return new Vector2(u, v);
    }
}
//public void EraseAtUV(Vector2 uv)
//{
//    Vector3 worldPos = UVToWorld(uv);
//    EraseAtWorldPosition(worldPos);
//}

//public void EraseAreaCompletely(Vector3 worldPos, float radius)
//{
//    float originalRadius = eraseRadius;
//    float originalStrength = eraseStrength;

//    eraseRadius = radius;
//    eraseStrength = 1f;

//    for (int i = 0; i < 5; i++)
//    {
//        EraseAtWorldPosition(worldPos);
//    }

//    eraseRadius = originalRadius;
//    eraseStrength = originalStrength;

//    if (isDirty)
//    {
//        maskTexture.Apply();
//        isDirty = false;
//    }
//}

//public void ResetMask()
//{
//    for (int i = 0; i < clearColors.Length; i++)
//    {
//        clearColors[i] = Color.white;
//    }
//    maskTexture.SetPixels(clearColors);
//    maskTexture.Apply();
//    isDirty = false;
//}

//public float GetErasePercentage()
//{
//    Color[] pixels = maskTexture.GetPixels();
//    float totalAlpha = 0f;

//    for (int i = 0; i < pixels.Length; i++)
//    {
//        totalAlpha += pixels[i].a;
//    }

//    return 1f - (totalAlpha / pixels.Length);
//}
//private Vector3 UVToWorld(Vector2 uv)
//{
//    Vector3 min = groundBounds.min;
//    Vector3 max = groundBounds.max;

//    float worldX = Mathf.Lerp(min.x, max.x, uv.x);
//    float worldY = Mathf.Lerp(min.y, max.y, uv.y); // 2D için Y ekseni

//    return new Vector3(worldX, worldY, transform.position.z); // Z'yi koru
//}

//public void RecalculateBounds()
//{
//    CalculateGroundBounds();
//}