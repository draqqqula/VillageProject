using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SkyboxMaskController : MonoBehaviour
{
    [SerializeField] private int _defaultRendererIndex;
    [SerializeField] private int _rendererIndex;
    [SerializeField] private Vector2 _resolutionScale = new Vector2(0.2f, 0.2f);
    [SerializeField] private Material _targetMaterial;
    [SerializeField] private Camera _camera;
    private RenderTexture _texture;
    private UniversalAdditionalCameraData _cameraData;
    private int _cachedPixelWidth;
    private int _cachedPixelHeight;

    private static RenderTexture Create2DRT(
    string name,
    int width,
    int height,
    GraphicsFormat format,
    FilterMode filterMode)
    {
        var desc = new RenderTextureDescriptor(width, height)
        {
            dimension = TextureDimension.Tex2D,
            graphicsFormat = format,
            depthStencilFormat = GraphicsFormat.D16_UNorm,
            msaaSamples = 1,
            useMipMap = false,
            autoGenerateMips = false
        };

        var rt = new RenderTexture(desc)
        {
            name = name,
            filterMode = filterMode,
            wrapMode = TextureWrapMode.Clamp
        };

        rt.Create();
        return rt;
    }

    private void Start()
    {
        _cameraData = _camera.GetUniversalAdditionalCameraData();

        RecreateTexture();
        _cachedPixelWidth = _camera.scaledPixelWidth;
        _cachedPixelHeight = _camera.scaledPixelHeight;
    }

    void LateUpdate()
    {
        if (_camera.scaledPixelWidth != _cachedPixelWidth || _camera.scaledPixelHeight != _cachedPixelHeight)
        {
            RecreateTexture();
            _cachedPixelWidth = _camera.scaledPixelWidth;
            _cachedPixelHeight = _camera.scaledPixelHeight;
        }
        //Render();
    }

    private void RecreateTexture()
    {
        var width = Mathf.RoundToInt(_camera.scaledPixelWidth * _resolutionScale.x);
        var height = Mathf.RoundToInt(_camera.scaledPixelHeight * _resolutionScale.y);

        _texture = Create2DRT(
            "Skybox Mask Raw",
            width,
            height,
            GraphicsFormat.R8_UNorm,
            FilterMode.Point
        );

        _targetMaterial.SetTexture("_SkyboxMask", _texture);

        RenderTexture.active = _texture;
        GL.Clear(true, true, new Color(0, 0, 0, 1));
        RenderTexture.active = null;
    }

    private void Render()
    {
        _cameraData.SetRenderer(_rendererIndex);
        _camera.targetTexture = _texture;
        _camera.Render();
        _camera.targetTexture = null;
        _cameraData.SetRenderer(_defaultRendererIndex);
    }
}