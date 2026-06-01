using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PixelatedRenderTexture : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera renderCamera;
    [SerializeField] private RawImage outputImage;

    [Header("Low Resolution")]
    [Min(16)]
    [SerializeField] private int pixelHeight = 180;

    [SerializeField] private bool matchScreenAspect = true;

    [Min(16)]
    [SerializeField] private int pixelWidth = 320;

    [Header("Screen")]
    [SerializeField] private bool fillScreen = true;

    private RenderTexture renderTexture;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Reset()
    {
        renderCamera = Camera.main;
        outputImage = GetComponent<RawImage>();
    }

    private void OnEnable()
    {
        CreateRenderTexture();
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            CreateRenderTexture();
        }
    }

    private void OnDisable()
    {
        if (renderCamera != null && renderCamera.targetTexture == renderTexture)
        {
            renderCamera.targetTexture = null;
        }

        if (outputImage != null && outputImage.texture == renderTexture)
        {
            outputImage.texture = null;
        }

        ReleaseRenderTexture();
    }

    private void CreateRenderTexture()
    {
        if (renderCamera == null || outputImage == null)
        {
            Debug.LogWarning("PixelatedRenderTexture: назначь Camera и RawImage.");
            return;
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        int height = Mathf.Max(16, pixelHeight);
        int width = matchScreenAspect
            ? Mathf.RoundToInt(height * (Screen.width / (float)Screen.height))
            : Mathf.Max(16, pixelWidth);

        ReleaseRenderTexture();

        renderTexture = new RenderTexture(width, height, 24)
        {
            name = "Pixelated Render Texture",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            antiAliasing = 1,
            useMipMap = false,
            autoGenerateMips = false
        };

        renderTexture.Create();

        renderCamera.targetTexture = renderTexture;
        outputImage.texture = renderTexture;

        if (fillScreen)
        {
            RectTransform rect = outputImage.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }

    private void ReleaseRenderTexture()
    {
        if (renderTexture == null)
            return;

        renderTexture.Release();
        Destroy(renderTexture);
        renderTexture = null;
    }
}