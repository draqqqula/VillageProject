using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;


public class CubemapRenderer : MonoBehaviour
{
    [SerializeField] private Camera _captureCamera;
    [SerializeField] private Transform _worldPosition;
    [SerializeField] private Material _cubemapMaterial;
    [SerializeField] private Material _equirectMaterial;
    [SerializeField] private Material _depthCubemapMaterial;
    [SerializeField] private Material _depthCopyMaterial;
    [SerializeField] private Material _texelMarchingMaterial;
    [SerializeField] private Material _radialDepthMaterial;
    [SerializeField] private Shader _radialDepthShader;
    [SerializeField] private int _cubemapWidth;
    [SerializeField] private int _cubemapHeight;
    [SerializeField] private int _depth;
    [SerializeField] private float _depthMax = 100f;

    private UniversalAdditionalCameraData _cameraData;
    private RenderTexture _cubeMap;
    private RenderTexture _cubeDepth;
    private RenderTexture _equirect;
    private RenderTexture _faceTexture;
    private RenderTexture _depthFaceTexture;

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
    void Start()
    {
        _cameraData = _captureCamera.GetUniversalAdditionalCameraData();

        GraphicsFormat colorFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);

        // ВАЖНО:
        // Эти RenderTexture используются как Camera.targetTexture,
        // поэтому у них depthStencilFormat должен быть не None.
        GraphicsFormat cameraDepthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt;

        // COLOR cubemap — storage texture, depth не нужен
        _cubeMap = CreateRT(
            "_CubeMap",
            _cubemapWidth,
            _cubemapHeight,
            TextureDimension.Cube,
            colorFormat,
            GraphicsFormat.None,
            FilterMode.Point
        );

        // DEPTH cubemap, depth stored as color — depth не нужен
        _cubeDepth = CreateRT(
            "_CubeDepth",
            _cubemapWidth,
            _cubemapHeight,
            TextureDimension.Cube,
            GraphicsFormat.R32_SFloat,
            GraphicsFormat.None,
            FilterMode.Point
        );

        // DEPTH face texture — используется как Camera.targetTexture,
        // поэтому нужен depth/stencil buffer, даже если глубина пишется в color RFloat
        _depthFaceTexture = CreateRT(
            "_DepthFaceTexture",
            _cubemapWidth,
            _cubemapHeight,
            TextureDimension.Tex2D,
            GraphicsFormat.R32_SFloat,
            cameraDepthStencilFormat,
            FilterMode.Point
        );

        // equirect — storage texture, depth не нужен
        _equirect = CreateRT(
            "_Equirect",
            _cubemapWidth,
            _cubemapHeight,
            TextureDimension.Tex2D,
            colorFormat,
            GraphicsFormat.None,
            FilterMode.Point
        );

        // face buffer — используется как Camera.targetTexture,
        // поэтому нужен depth/stencil buffer
        _faceTexture = CreateRT(
            "_FaceTexture",
            _cubemapWidth,
            _cubemapHeight,
            TextureDimension.Tex2D,
            colorFormat,
            cameraDepthStencilFormat,
            FilterMode.Point
        );

        _cubemapMaterial.SetTexture("_Cube", _cubeMap);
        _equirectMaterial.SetTexture("_Tex", _equirect);
        _depthCubemapMaterial.SetTexture("_Cube", _cubeDepth);

        _captureCamera.aspect = 1f;
        _captureCamera.fieldOfView = 90f;
        _captureCamera.targetTexture = _faceTexture;
        _captureCamera.depthTextureMode = DepthTextureMode.Depth;

        _texelMarchingMaterial.SetFloat("_DepthMax", 100f);
        _texelMarchingMaterial.SetInt("_MaxSteps", 116);
    }
    void LateUpdate()
    {
        RenderCubemaps();

        UpdateCubemapOffset();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetProbe();
        }
    }

    void SetProbe()
    {
        Debug.Log("Probe updated");
        _texelMarchingMaterial.SetTexture("_ColorCube", _cubeMap);
        _texelMarchingMaterial.SetTexture("_DepthCube", _cubeDepth);

        _captureCamera.transform.position = _worldPosition.position;
    }

    void UpdateCubemapOffset()
    {
        var offset = _worldPosition.position - _captureCamera.transform.position;
        _texelMarchingMaterial.SetVector("_CubemapOffset", offset);
    }

    void RenderCubemaps()
    {
        Vector3 origin = _captureCamera.transform.position;

        _radialDepthMaterial.SetVector("_CubemapOrigin", origin);
        _radialDepthMaterial.SetFloat("_DepthMax", _depthMax);

        for (int face = 0; face < 6; face++)
        {
            var cubemapFace = (CubemapFace)face;

            _captureCamera.transform.rotation = GetFaceRotation(cubemapFace);

            Matrix4x4 view = _captureCamera.worldToCameraMatrix;
            Matrix4x4 flip = GetFaceMatrix(cubemapFace);

            GL.invertCulling = true;
            _captureCamera.worldToCameraMatrix = flip * view;

            // ---------- COLOR PASS ----------
            _captureCamera.targetTexture = _faceTexture;
            _cameraData.SetRenderer(0);
            _captureCamera.backgroundColor = Color.magenta;
            _captureCamera.Render();

            Graphics.CopyTexture(_faceTexture, 0, 0, _cubeMap, face, 0);

            // ---------- DEPTH PASS ----------

            _captureCamera.targetTexture = _depthFaceTexture;
            _cameraData.SetRenderer(3);
            _captureCamera.backgroundColor = Color.red;
            _captureCamera.Render();

            Graphics.CopyTexture(_depthFaceTexture, 0, 0, _cubeDepth, face, 0);

            GL.invertCulling = false;
            _captureCamera.ResetWorldToCameraMatrix();
        }
    }

    Quaternion GetFaceRotation(CubemapFace face)
    {
        switch (face)
        {
            case CubemapFace.PositiveX:
                return Quaternion.LookRotation(Vector3.right, Vector3.down);

            case CubemapFace.NegativeX:
                return Quaternion.LookRotation(Vector3.left, Vector3.down);

            case CubemapFace.PositiveY:
                return Quaternion.LookRotation(Vector3.up, Vector3.forward);

            case CubemapFace.NegativeY:
                return Quaternion.LookRotation(Vector3.down, Vector3.back);

            case CubemapFace.PositiveZ:
                return Quaternion.LookRotation(Vector3.forward, Vector3.down);

            case CubemapFace.NegativeZ:
                return Quaternion.LookRotation(Vector3.back, Vector3.down);
        }

        return Quaternion.identity;
    }

    Matrix4x4 GetFaceMatrix(CubemapFace face)
    {
        // horizontal flip (mirror X)
        return Matrix4x4.Scale(new Vector3(-1, 1, 1));
    }
}