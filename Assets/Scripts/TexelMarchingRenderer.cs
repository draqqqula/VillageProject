using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TexelMarchingRenderer : MonoBehaviour
{
    private const int DefaultRenderer = 0;
    private const int DepthRenderer = 3;

    private const int AllCubemapFacesMask = 0b111111;
    private const float BlendEpsilon = 0.0001f;

    // acos(1 / sqrt(3)) = угол от face normal до угла cubemap face.
    private const float CubemapFaceCornerAngleDegrees = 54.73561f;

    [Header("Cameras")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _captureCamera;

    [Header("Probe Source")]
    [Tooltip("Опционально. Если не задан, будет использован GetComponent<IProbeBlendProvider>().")]
    [SerializeField] private MonoBehaviour _probeBlendProviderBehaviour;

    [Header("Rendering")]
    [SerializeField] private Transform _worldPoint;
    [SerializeField] private Material _texelMarchingMaterial;
    [SerializeField] private Material _depthMaterial;
    [SerializeField] private Vector2Int _cubemapResolution = new Vector2Int(128, 128);

    [Header("Depth / Clip Planes")]
    [SerializeField, Min(0.001f)] private float _depthNear = 0.1f;
    [SerializeField, Min(0.001f)] private float _depthFar = 100f;

    [Header("Cubemap Face Culling")]
    [SerializeField] private bool _enableFaceCulling = true;

    [Tooltip("Сколько точек по ширине/высоте viewport использовать для оценки cubemap face mask.")]
    [SerializeField, Range(2, 7)] private int _faceMaskViewportSamplesPerAxis = 5;

    [Tooltip("Запас в градусах. Увеличь, если видишь popping на границах cubemap faces.")]
    [SerializeField, Range(0f, 30f)] private float _faceMaskPaddingDegrees = 8f;

    [Header("Probe Render Cache")]
    [Tooltip("true — обновлять видимые faces probe каждый кадр. Нужно для динамических объектов.")]
    [SerializeField] private bool _renderStaticProbesEveryFrame = false;

    [Tooltip("true — обновлять fallback каждый кадр. Обычно fallback следует за камерой, поэтому это безопасный дефолт.")]
    [SerializeField] private bool _renderFallbackEveryFrame = true;

    [SerializeField] private float _probePositionEpsilon = 0.0001f;

    [Header("Debug")]
    [SerializeField] private bool _logRenderToCubemapFailures = false;

    private IProbeBlendProvider _probeBlendProvider;
    private UniversalAdditionalCameraData _captureCameraData;

    private RenderTexture _cubemapColorTextureA;
    private RenderTexture _cubemapDepthTextureA;
    private RenderTexture _cubemapColorTextureB;
    private RenderTexture _cubemapDepthTextureB;
    private RenderTexture _cubemapColorTextureFallback;

    private Vector3 _probeA;
    private Vector3 _probeB;

    private GraphicsFormat _colorFormat;
    private GraphicsFormat _depthFormat;

    private int _blendProperty;
    private int _offsetPingProperty;
    private int _offsetPongProperty;
    private int _depthNearProperty;
    private int _depthFarProperty;

    private float _currentBlend;

    private bool _hasRenderedProbeA;
    private bool _hasRenderedProbeB;
    private bool _hasRenderedFallback;

    private Vector3 _renderedProbeAPosition;
    private Vector3 _renderedProbeBPosition;
    private Vector3 _renderedFallbackPosition;

    private int _renderedProbeAFaceMask;
    private int _renderedProbeBFaceMask;
    private int _renderedFallbackFaceMask;

    private float DepthNear => Mathf.Max(0.001f, _depthNear);
    private float DepthFar => Mathf.Max(DepthNear + 0.001f, _depthFar);


    private void Start()
    {
        ResolveReferences();

        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        _colorFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
        _depthFormat = GraphicsFormat.R16_UNorm;

        InitializeCamera();
        InitializeShaderProperties();
        InitializeTextures();

        ApplyDepthRangeToMaterials();
    }

    private void LateUpdate()
    {
        if (_probeBlendProvider == null)
            return;

        UpdateProbes();

        int probeAMask = GetCubemapMask(_probeA);
        int probeBMask = GetCubemapMask(_probeB);
        int fallbackMask = GetCubemapMask(_worldPoint.position);

        if (_currentBlend <= BlendEpsilon)
        {
            // Shader использует только Ping.
            EnsureProbeRendered(
                _probeA,
                _cubemapColorTextureA,
                _cubemapDepthTextureA,
                probeAMask,
                ref _hasRenderedProbeA,
                ref _renderedProbeAPosition,
                ref _renderedProbeAFaceMask
            );
        }
        else if (_currentBlend >= 1f - BlendEpsilon)
        {
            // Shader использует только Pong.
            EnsureProbeRendered(
                _probeB,
                _cubemapColorTextureB,
                _cubemapDepthTextureB,
                probeBMask,
                ref _hasRenderedProbeB,
                ref _renderedProbeBPosition,
                ref _renderedProbeBFaceMask
            );
        }
        else
        {
            // Только во время реального crossfade нужны оба cubemap.
            EnsureProbeRendered(
                _probeA,
                _cubemapColorTextureA,
                _cubemapDepthTextureA,
                probeAMask,
                ref _hasRenderedProbeA,
                ref _renderedProbeAPosition,
                ref _renderedProbeAFaceMask
            );

            EnsureProbeRendered(
                _probeB,
                _cubemapColorTextureB,
                _cubemapDepthTextureB,
                probeBMask,
                ref _hasRenderedProbeB,
                ref _renderedProbeBPosition,
                ref _renderedProbeBFaceMask
            );
        }

        EnsureFallbackRendered(fallbackMask);
    }

    private void ResolveReferences()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_worldPoint == null && _mainCamera != null)
            _worldPoint = _mainCamera.transform;

        if (_probeBlendProviderBehaviour != null)
            _probeBlendProvider = _probeBlendProviderBehaviour as IProbeBlendProvider;

        if (_probeBlendProvider == null)
            _probeBlendProvider = GetComponent<IProbeBlendProvider>();
    }

    private bool ValidateReferences()
    {
        bool ok = true;

        if (_mainCamera == null)
        {
            Debug.LogError($"{nameof(TexelMarchingRenderer)}: Main Camera is not assigned.", this);
            ok = false;
        }

        if (_captureCamera == null)
        {
            Debug.LogError($"{nameof(TexelMarchingRenderer)}: Capture Camera is not assigned.", this);
            ok = false;
        }

        if (_worldPoint == null)
        {
            Debug.LogError($"{nameof(TexelMarchingRenderer)}: World Point is not assigned.", this);
            ok = false;
        }

        if (_texelMarchingMaterial == null)
        {
            Debug.LogError($"{nameof(TexelMarchingRenderer)}: Texel Marching Material is not assigned.", this);
            ok = false;
        }

        if (_depthMaterial == null)
        {
            Debug.LogError($"{nameof(TexelMarchingRenderer)}: Depth Material is not assigned.", this);
            ok = false;
        }

        if (_probeBlendProvider == null)
        {
            Debug.LogError(
                $"{nameof(TexelMarchingRenderer)}: IProbeBlendProvider was not found. " +
                $"Assign Probe Blend Provider Behaviour or add a component implementing IProbeBlendProvider.",
                this
            );
            ok = false;
        }

        if (_cubemapResolution.x <= 0 || _cubemapResolution.y <= 0)
        {
            Debug.LogError($"{nameof(TexelMarchingRenderer)}: Cubemap resolution must be positive.", this);
            ok = false;
        }

        return ok;
    }

    private void ApplyDepthRangeToCamera()
    {
        float near = DepthNear;
        float far = DepthFar;

        _captureCamera.nearClipPlane = near;
        _captureCamera.farClipPlane = far;
    }

    private void ApplyDepthRangeToMaterials()
    {
        float near = DepthNear;
        float far = DepthFar;

        _texelMarchingMaterial.SetFloat(_depthNearProperty, near);
        _texelMarchingMaterial.SetFloat(_depthFarProperty, far);

        _depthMaterial.SetFloat(_depthNearProperty, near);
        _depthMaterial.SetFloat(_depthFarProperty, far);
    }

    private void InitializeCamera()
    {
        _captureCamera.enabled = false;
        _captureCamera.aspect = 1f;
        _captureCamera.fieldOfView = 90f;
        _captureCamera.depthTextureMode = DepthTextureMode.None;

        ApplyDepthRangeToCamera();

        _captureCameraData = _captureCamera.GetUniversalAdditionalCameraData();
    }

    private void InitializeShaderProperties()
    {
        _blendProperty = Shader.PropertyToID("_Blend");
        _offsetPingProperty = Shader.PropertyToID("_CubemapOffsetPing");
        _offsetPongProperty = Shader.PropertyToID("_CubemapOffsetPong");
        _depthNearProperty = Shader.PropertyToID("_DepthNear");
        _depthFarProperty = Shader.PropertyToID("_DepthFar");
    }

    private void InitializeTextures()
    {
        _cubemapColorTextureA = InitializeCubemapRT("_ColorCubePing", _cubemapResolution, false);
        _cubemapDepthTextureA = InitializeCubemapRT("_DepthCubePing", _cubemapResolution, true);

        _cubemapColorTextureB = InitializeCubemapRT("_ColorCubePong", _cubemapResolution, false);
        _cubemapDepthTextureB = InitializeCubemapRT("_DepthCubePong", _cubemapResolution, true);

        _cubemapColorTextureFallback = InitializeCubemapRT("_FallbackCubePing", _cubemapResolution, false);

        // В текущем шейдере fallback Ping/Pong может указывать на одну и ту же актуальную cubemap.
        _texelMarchingMaterial.SetTexture("_FallbackCubePong", _cubemapColorTextureFallback);
    }

    private RenderTexture InitializeCubemapRT(
        string property,
        Vector2Int resolution,
        bool isDepthTexture)
    {
        RenderTexture texture = CreateRT(
            property,
            resolution.x,
            resolution.y,
            TextureDimension.Cube,
            isDepthTexture ? _depthFormat : _colorFormat,
            GraphicsFormat.None,
            FilterMode.Point
        );

        _texelMarchingMaterial.SetTexture(property, texture);
        return texture;
    }

    private static RenderTexture CreateRT(
        string name,
        int width,
        int height,
        TextureDimension dimension,
        GraphicsFormat colorFormat,
        GraphicsFormat depthStencilFormat,
        FilterMode filterMode)
    {
        var desc = new RenderTextureDescriptor(width, height)
        {
            dimension = dimension,
            graphicsFormat = colorFormat,
            depthStencilFormat = depthStencilFormat,
            msaaSamples = 1,
            useMipMap = false,
            autoGenerateMips = false
        };

        if (dimension == TextureDimension.Cube)
            desc.volumeDepth = 6;

        var rt = new RenderTexture(desc)
        {
            name = name,
            filterMode = filterMode,
            wrapMode = TextureWrapMode.Clamp
        };

        rt.Create();
        return rt;
    }

    private void UpdateProbes()
    {
        _probeA = _probeBlendProvider.ProbeA;
        _probeB = _probeBlendProvider.ProbeB;

        _currentBlend = Mathf.Clamp01(_probeBlendProvider.Blend);

        // Схлопываем почти-0 и почти-1, чтобы и shader, и CPU попадали в fast path.
        if (_currentBlend <= BlendEpsilon)
            _currentBlend = 0f;
        else if (_currentBlend >= 1f - BlendEpsilon)
            _currentBlend = 1f;

        _texelMarchingMaterial.SetFloat(_blendProperty, _currentBlend);
        ApplyDepthRangeToMaterials();
        ApplyDepthRangeToCamera();

        // Offset должен быть current viewer position - cubemap capture position.
        _texelMarchingMaterial.SetVector(
            _offsetPingProperty,
            _worldPoint.position - _probeA
        );

        _texelMarchingMaterial.SetVector(
            _offsetPongProperty,
            _worldPoint.position - _probeB
        );
    }

    private void RenderCubemapOnPosition(
        Vector3 position,
        RenderTexture color,
        RenderTexture depth,
        int faceMask)
    {
        faceMask &= AllCubemapFacesMask;

        if (faceMask == 0 || color == null)
            return;

        bool oldInvertCulling = GL.invertCulling;

        try
        {
            GL.invertCulling = true;

            _captureCamera.transform.position = position;
            ApplyDepthRangeToCamera();

            _captureCamera.clearFlags = CameraClearFlags.SolidColor;
            _captureCamera.backgroundColor = Color.magenta;
            //_captureCamera.clearFlags = CameraClearFlags.Skybox;
            _captureCameraData.SetRenderer(DefaultRenderer);

            bool colorOk = _captureCamera.RenderToCubemap(color, faceMask);
            if (!colorOk && _logRenderToCubemapFailures)
            {
                Debug.LogWarning(
                    $"{nameof(TexelMarchingRenderer)}: RenderToCubemap color failed. FaceMask: {faceMask}",
                    this
                );
            }

            if (depth != null)
            {
                _depthMaterial.SetVector("_CubemapOrigin", position);
                _depthMaterial.SetFloat(_depthNearProperty, DepthNear);
                _depthMaterial.SetFloat(_depthFarProperty, DepthFar);

                _captureCamera.clearFlags = CameraClearFlags.SolidColor;
                _captureCamera.backgroundColor = Color.red;
                _captureCameraData.SetRenderer(DepthRenderer);

                bool depthOk = _captureCamera.RenderToCubemap(depth, faceMask);
                if (!depthOk && _logRenderToCubemapFailures)
                {
                    Debug.LogWarning(
                        $"{nameof(TexelMarchingRenderer)}: RenderToCubemap depth failed. FaceMask: {faceMask}",
                        this
                    );
                }
            }
        }
        finally
        {
            GL.invertCulling = oldInvertCulling;
        }
    }

    private void EnsureProbeRendered(
        Vector3 position,
        RenderTexture color,
        RenderTexture depth,
        int requiredFaceMask,
        ref bool hasRendered,
        ref Vector3 renderedPosition,
        ref int renderedFaceMask)
    {
        requiredFaceMask &= AllCubemapFacesMask;

        if (requiredFaceMask == 0)
            return;

        bool positionChanged =
            !hasRendered ||
            !IsSamePosition(position, renderedPosition);

        int faceMaskToRender;

        if (positionChanged)
        {
            // Probe переместилась: старые faces больше невалидны.
            renderedFaceMask = 0;
            faceMaskToRender = requiredFaceMask;
        }
        else if (_renderStaticProbesEveryFrame)
        {
            // Для динамической сцены обновляем видимые faces каждый кадр.
            faceMaskToRender = requiredFaceMask;
        }
        else
        {
            // Для статической сцены дорендериваем только faces,
            // которые стали видимыми после поворота камеры.
            faceMaskToRender = requiredFaceMask & ~renderedFaceMask;

            if (faceMaskToRender == 0)
                return;
        }

        RenderCubemapOnPosition(position, color, depth, faceMaskToRender);

        renderedPosition = position;
        renderedFaceMask |= faceMaskToRender;
        hasRendered = true;
    }

    private void EnsureFallbackRendered(int requiredFaceMask)
    {
        requiredFaceMask &= AllCubemapFacesMask;

        if (requiredFaceMask == 0)
            return;

        Vector3 position = _worldPoint.position;

        bool positionChanged =
            !_hasRenderedFallback ||
            !IsSamePosition(position, _renderedFallbackPosition);

        int faceMaskToRender;

        if (positionChanged)
        {
            _renderedFallbackFaceMask = 0;
            faceMaskToRender = requiredFaceMask;
        }
        else if (_renderFallbackEveryFrame)
        {
            faceMaskToRender = requiredFaceMask;
        }
        else
        {
            faceMaskToRender = requiredFaceMask & ~_renderedFallbackFaceMask;

            if (faceMaskToRender == 0)
                return;
        }

        RenderCubemapOnPosition(
            position,
            _cubemapColorTextureFallback,
            null,
            faceMaskToRender
        );

        _renderedFallbackPosition = position;
        _renderedFallbackFaceMask |= faceMaskToRender;
        _hasRenderedFallback = true;
    }

    private int GetCubemapMask(Vector3 cubemapPosition)
    {
        if (!_enableFaceCulling || _mainCamera == null)
            return AllCubemapFacesMask;

        int mask = 0;

        Vector3 cameraPosition = _mainCamera.transform.position;

        // Учитываем offset от cubemap origin к текущей камере.
        // Если cubemap стоит прямо в камере, этот вектор нулевой — пропускаем,
        // иначе fallback при camera-position capture всегда рендерил бы все faces.
        AddFacesForDirection(cameraPosition - cubemapPosition, ref mask);

        int samplesPerAxis = Mathf.Max(2, _faceMaskViewportSamplesPerAxis);

        float nearDistance = Mathf.Max(_mainCamera.nearClipPlane, DepthNear, 0.01f);
        float farDistance = Mathf.Min(
            Mathf.Max(_mainCamera.farClipPlane, nearDistance + 0.01f),
            Mathf.Max(DepthFar, nearDistance + 0.01f)
        );

        if (farDistance <= nearDistance)
            farDistance = nearDistance + 0.01f;

        float midDistance = Mathf.Sqrt(nearDistance * farDistance);

        for (int y = 0; y < samplesPerAxis; y++)
        {
            float v = samplesPerAxis == 1
                ? 0.5f
                : y / (float)(samplesPerAxis - 1);

            for (int x = 0; x < samplesPerAxis; x++)
            {
                float u = samplesPerAxis == 1
                    ? 0.5f
                    : x / (float)(samplesPerAxis - 1);

                Ray ray = _mainCamera.ViewportPointToRay(new Vector3(u, v, 0f));

                AddFacesForDirection(ray.origin + ray.direction * nearDistance - cubemapPosition, ref mask);
                AddFacesForDirection(ray.origin + ray.direction * midDistance - cubemapPosition, ref mask);
                AddFacesForDirection(ray.origin + ray.direction * farDistance - cubemapPosition, ref mask);
            }
        }

        // Safety fallback: если по какой-то причине ничего не нашли, лучше рендерить всё,
        // чем получить stale faces.
        return mask == 0 ? AllCubemapFacesMask : mask & AllCubemapFacesMask;
    }

    private void AddFacesForDirection(Vector3 direction, ref int mask)
    {
        float sqrMagnitude = direction.sqrMagnitude;

        if (sqrMagnitude < 0.0000001f)
            return;

        Vector3 d = direction / Mathf.Sqrt(sqrMagnitude);

        float maxAngle = Mathf.Min(
            89f,
            CubemapFaceCornerAngleDegrees + _faceMaskPaddingDegrees
        );

        float cosLimit = Mathf.Cos(maxAngle * Mathf.Deg2Rad);

        if (Vector3.Dot(d, Vector3.right) >= cosLimit)
            mask |= 1 << (int)CubemapFace.PositiveX;

        if (Vector3.Dot(d, Vector3.left) >= cosLimit)
            mask |= 1 << (int)CubemapFace.NegativeX;

        if (Vector3.Dot(d, Vector3.up) >= cosLimit)
            mask |= 1 << (int)CubemapFace.PositiveY;

        if (Vector3.Dot(d, Vector3.down) >= cosLimit)
            mask |= 1 << (int)CubemapFace.NegativeY;

        if (Vector3.Dot(d, Vector3.forward) >= cosLimit)
            mask |= 1 << (int)CubemapFace.PositiveZ;

        if (Vector3.Dot(d, Vector3.back) >= cosLimit)
            mask |= 1 << (int)CubemapFace.NegativeZ;
    }

    private bool IsSamePosition(Vector3 a, Vector3 b)
    {
        float epsilon = Mathf.Max(_probePositionEpsilon, 0.000001f);
        return (a - b).sqrMagnitude <= epsilon * epsilon;
    }

    public void InvalidateProbeCache()
    {
        _hasRenderedProbeA = false;
        _hasRenderedProbeB = false;
        _hasRenderedFallback = false;

        _renderedProbeAFaceMask = 0;
        _renderedProbeBFaceMask = 0;
        _renderedFallbackFaceMask = 0;
    }

    private void OnDestroy()
    {
        ReleaseRT(ref _cubemapColorTextureA);
        ReleaseRT(ref _cubemapDepthTextureA);
        ReleaseRT(ref _cubemapColorTextureB);
        ReleaseRT(ref _cubemapDepthTextureB);
        ReleaseRT(ref _cubemapColorTextureFallback);
    }

    private static void ReleaseRT(ref RenderTexture rt)
    {
        if (rt == null)
            return;

        rt.Release();
        Destroy(rt);
        rt = null;
    }
}