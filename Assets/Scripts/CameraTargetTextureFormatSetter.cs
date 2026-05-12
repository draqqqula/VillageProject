using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class CameraTargetTextureFormatSetter : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _camera;

    [Header("Resolution")]
    [SerializeField] private bool _useCameraPixelSize = true;
    [SerializeField, Min(1)] private int _width = 1920;
    [SerializeField, Min(1)] private int _height = 1080;
    [SerializeField, Range(1, 8)] private int _downsample = 1;

    [Header("Format")]
    [SerializeField] private GraphicsFormat _colorFormat = GraphicsFormat.R8G8B8A8_UNorm;
    [SerializeField] private GraphicsFormat _depthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt;

    [Header("Sampling")]
    [SerializeField] private FilterMode _filterMode = FilterMode.Point;
    [SerializeField] private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;
    [SerializeField] private bool _useMipMap = false;
    [SerializeField] private bool _autoGenerateMips = false;
    [SerializeField, Range(1, 8)] private int _msaaSamples = 1;

    [Header("Output")]
    [SerializeField] private string _textureName = "Camera Target Texture";
    [SerializeField] private bool _clearTargetTextureOnDisable = true;

    public RenderTexture TargetTexture => _targetTexture;

    private RenderTexture _targetTexture;

    private int _lastWidth;
    private int _lastHeight;
    private int _lastDownsample;
    private GraphicsFormat _lastColorFormat;
    private GraphicsFormat _lastDepthStencilFormat;
    private FilterMode _lastFilterMode;
    private TextureWrapMode _lastWrapMode;
    private bool _lastUseMipMap;
    private bool _lastAutoGenerateMips;
    private int _lastMsaaSamples;

    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();

        Apply();
    }

    private void OnValidate()
    {
        _width = Mathf.Max(1, _width);
        _height = Mathf.Max(1, _height);
        _downsample = Mathf.Max(1, _downsample);
        _msaaSamples = Mathf.Max(1, _msaaSamples);

        if (isActiveAndEnabled)
            Apply();
    }

    private void LateUpdate()
    {
        Apply();
    }

    public void Apply()
    {
        if (_camera == null)
            return;

        int targetWidth;
        int targetHeight;

        if (_useCameraPixelSize)
        {
            targetWidth = Mathf.Max(1, _camera.pixelWidth / Mathf.Max(1, _downsample));
            targetHeight = Mathf.Max(1, _camera.pixelHeight / Mathf.Max(1, _downsample));
        }
        else
        {
            targetWidth = Mathf.Max(1, _width / Mathf.Max(1, _downsample));
            targetHeight = Mathf.Max(1, _height / Mathf.Max(1, _downsample));
        }

        bool needsRecreate =
            _targetTexture == null ||
            !_targetTexture.IsCreated() ||
            targetWidth != _lastWidth ||
            targetHeight != _lastHeight ||
            _downsample != _lastDownsample ||
            _colorFormat != _lastColorFormat ||
            _depthStencilFormat != _lastDepthStencilFormat ||
            _filterMode != _lastFilterMode ||
            _wrapMode != _lastWrapMode ||
            _useMipMap != _lastUseMipMap ||
            _autoGenerateMips != _lastAutoGenerateMips ||
            _msaaSamples != _lastMsaaSamples;

        if (!needsRecreate)
        {
            if (_camera.targetTexture != _targetTexture)
                _camera.targetTexture = _targetTexture;

            return;
        }

        RecreateTargetTexture(targetWidth, targetHeight);
    }

    private void RecreateTargetTexture(int width, int height)
    {
        ReleaseTargetTexture();

        var descriptor = new RenderTextureDescriptor(width, height)
        {
            dimension = TextureDimension.Tex2D,
            volumeDepth = 1,

            // ¬ажна€ строка: именно здесь выбираетс€ формат color target.
            graphicsFormat = _colorFormat,

            // ћожно поставить GraphicsFormat.None, если depth не нужен.
            depthStencilFormat = _depthStencilFormat,

            msaaSamples = Mathf.Max(1, _msaaSamples),
            useMipMap = _useMipMap,
            autoGenerateMips = _autoGenerateMips,
            sRGB = GraphicsFormatUtility.IsSRGBFormat(_colorFormat)
        };

        _targetTexture = new RenderTexture(descriptor)
        {
            name = _textureName,
            filterMode = _filterMode,
            wrapMode = _wrapMode
        };

        _targetTexture.Create();

        _camera.targetTexture = _targetTexture;

        _lastWidth = width;
        _lastHeight = height;
        _lastDownsample = _downsample;
        _lastColorFormat = _colorFormat;
        _lastDepthStencilFormat = _depthStencilFormat;
        _lastFilterMode = _filterMode;
        _lastWrapMode = _wrapMode;
        _lastUseMipMap = _useMipMap;
        _lastAutoGenerateMips = _autoGenerateMips;
        _lastMsaaSamples = _msaaSamples;
    }

    public void SetFormat(GraphicsFormat colorFormat)
    {
        _colorFormat = colorFormat;
        Apply();
    }

    public void SetFormat(GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat)
    {
        _colorFormat = colorFormat;
        _depthStencilFormat = depthStencilFormat;
        Apply();
    }

    public void SetResolution(int width, int height, int downsample = 1)
    {
        _useCameraPixelSize = false;
        _width = Mathf.Max(1, width);
        _height = Mathf.Max(1, height);
        _downsample = Mathf.Max(1, downsample);
        Apply();
    }

    public void UseCameraResolution(int downsample = 1)
    {
        _useCameraPixelSize = true;
        _downsample = Mathf.Max(1, downsample);
        Apply();
    }

    private void OnDisable()
    {
        if (_clearTargetTextureOnDisable && _camera != null && _camera.targetTexture == _targetTexture)
            _camera.targetTexture = null;

        ReleaseTargetTexture();
    }

    private void OnDestroy()
    {
        if (_camera != null && _camera.targetTexture == _targetTexture)
            _camera.targetTexture = null;

        ReleaseTargetTexture();
    }

    private void ReleaseTargetTexture()
    {
        if (_targetTexture == null)
            return;

        if (_camera != null && _camera.targetTexture == _targetTexture)
            _camera.targetTexture = null;

        _targetTexture.Release();

        if (Application.isPlaying)
            Destroy(_targetTexture);
        else
            DestroyImmediate(_targetTexture);

        _targetTexture = null;
    }
}