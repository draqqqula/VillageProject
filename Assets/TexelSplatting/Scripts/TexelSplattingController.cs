using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TexelSplatting
{
    [RequireComponent(typeof(Camera))]
    public class TexelSplattingController : MonoBehaviour
    {
        [Header("Shaders")]
        [SerializeField] private Shader _splatShader;
        [SerializeField] private Shader _linearizeDepthShader;
        [SerializeField] private Shader _compositeShader;

        [Header("Settings")]
        [SerializeField] private int _faceResolution = 256;
        [SerializeField] private int _posterizeLevels = 24;
        [SerializeField] private float _probeGridSpacing = 1f;
        [SerializeField] private float _splatSizeMultiplier = 1.5f;

        public static TexelSplattingController Instance { get; private set; }
        
        public Material LinearizeDepthMat => _linearizeDepthMaterial;
        public Material CompositeMat => _compositeMaterial;
        public Camera MainCamera => _mainCamera;

        private Camera _mainCamera;
        private ComputeShader _splatCompute;
        private int _computeKernel;
        private Material _splatMaterial;
        private Material _linearizeDepthMaterial;
        private Material _compositeMaterial;

        private Dictionary<Camera, int> _faceCameraLookup = new();
        private Camera[] _faceCameras;
        private RenderTexture[] _colorFaces;
        private RenderTexture[] _depthFaces;
        private RenderTexture _colorArray;
        private RenderTexture _depthArray;

        private Camera _cubemapCamera;
        private RenderTexture _cameraCubemapRT;

        private RenderTexture _splatRT;
        private RenderTexture _splatDepthRT;
        private ComputeBuffer _splatBuffer;
        private ComputeBuffer _argsBuffer;
        private Mesh _quadMesh;

        private Matrix4x4[] _faceInvViewArray = new Matrix4x4[6];
        private uint _activeFaceMask;
        private uint _renderedFaceMask;
        private uint _freshlyRenderedMask;
        private Vector3 _previousGridCenter = new(float.NaN, float.NaN, float.NaN);

        static class ShaderIDs
        {
            public static readonly int FaceResolution = Shader.PropertyToID("_FaceResolution");
            public static readonly int FarPlane = Shader.PropertyToID("_FarPlane");
            public static readonly int PosterizeLevels = Shader.PropertyToID("_PosterizeLevels");
            public static readonly int SplatSizeMultiplier = Shader.PropertyToID("_SplatSizeMultiplier");
            public static readonly int FaceIndices0 = Shader.PropertyToID("_FaceIndices0");
            public static readonly int FaceIndices1 = Shader.PropertyToID("_FaceIndices1");
            public static readonly int SplatBuffer = Shader.PropertyToID("_SplatBuffer");
            public static readonly int ProbeColorArray = Shader.PropertyToID("_ProbeColorArray");
            public static readonly int ProbeDepthArray = Shader.PropertyToID("_ProbeDepthArray");
            public static readonly int FaceInvViews = Shader.PropertyToID("_FaceInvViews");
            public static readonly int CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
            public static readonly int InvViewProjMatrix = Shader.PropertyToID("_TS_InvViewProjMatrix");
            public static readonly int CameraWorldPos = Shader.PropertyToID("_CameraWorldPos");
            public static readonly int CameraCubemap = Shader.PropertyToID("_CameraCubemap");
            public static readonly int[] FrustumPlanes = new int[6];

            static ShaderIDs()
            {
                for (int i = 0; i < 6; i++)
                    FrustumPlanes[i] = Shader.PropertyToID($"_FrustumPlane{i}");
            }
        }

        static readonly Plane[] _frustumPlanesCache = new Plane[6];

        static readonly Vector3[] FaceForward =
        {
            Vector3.right, Vector3.left,
            Vector3.up, Vector3.down,
            Vector3.forward, Vector3.back
        };

        static readonly Vector3[] FaceUp =
        {
            Vector3.up, Vector3.up,
            Vector3.back, Vector3.forward,
            Vector3.up, Vector3.up
        };

        void OnEnable()
        {
            Instance = this;
            _mainCamera = GetComponent<Camera>();
            _splatCompute = Resources.Load<ComputeShader>("TexelSplatGenerate");
            _computeKernel = _splatCompute.FindKernel("CSMain");

            CreateMaterials();
            CreateRenderTextures();
            CreateBuffers();
            CreateQuad();
            CreateFaceCameras();
            CreateCubemapCamera();
        }

        void OnDisable()
        {
            DestroyFaceCameras();
            DestroyCubemapCamera();
            ReleaseBuffers();
            ReleaseRenderTextures();
            DestroyMaterials();

            if (Instance == this) Instance = null;
        }

        void LateUpdate()
        {
            EnsureSplatRTSize();
            PositionProbe();
            RenderCameraCubemap();
        }

        void CreateMaterials()
        {
            _splatMaterial = new Material(_splatShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _linearizeDepthMaterial = new Material(_linearizeDepthShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _compositeMaterial = new Material(_compositeShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        void DestroyMaterials()
        {
            if (_splatMaterial != null) DestroyImmediate(_splatMaterial);
            if (_linearizeDepthMaterial != null) DestroyImmediate(_linearizeDepthMaterial);
            if (_compositeMaterial != null) DestroyImmediate(_compositeMaterial);
        }

        void CreateRenderTextures()
        {
            _colorFaces = new RenderTexture[6];
            _depthFaces = new RenderTexture[6];

            for (int i = 0; i < 6; i++)
            {
                _colorFaces[i] = new RenderTexture(_faceResolution, _faceResolution, 24, RenderTextureFormat.ARGB32)
                {
                    filterMode = FilterMode.Point
                };
                _colorFaces[i].Create();

                _depthFaces[i] = new RenderTexture(_faceResolution, _faceResolution, 0, RenderTextureFormat.RFloat)
                {
                    filterMode = FilterMode.Point
                };
                _depthFaces[i].Create();
            }

            _colorArray = new RenderTexture(_faceResolution, _faceResolution, 0, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Tex2DArray,
                volumeDepth = 6,
                filterMode = FilterMode.Point,
                enableRandomWrite = true
            };
            _colorArray.Create();

            _depthArray = new RenderTexture(_faceResolution, _faceResolution, 0, RenderTextureFormat.RFloat)
            {
                dimension = TextureDimension.Tex2DArray,
                volumeDepth = 6,
                filterMode = FilterMode.Point,
                enableRandomWrite = true
            };
            _depthArray.Create();

            int w = _mainCamera.pixelWidth > 0 ? _mainCamera.pixelWidth : Screen.width;
            int h = _mainCamera.pixelHeight > 0 ? _mainCamera.pixelHeight : Screen.height;

            _splatRT = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
            _splatRT.Create();

            _splatDepthRT = new RenderTexture(w, h, 24, RenderTextureFormat.Depth);
            _splatDepthRT.Create();

            _cameraCubemapRT = new RenderTexture(_faceResolution, _faceResolution, 24, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube,
                filterMode = FilterMode.Point
            };
            _cameraCubemapRT.Create();
        }

        void ReleaseRenderTextures()
        {
            if (_colorFaces != null)
                for (int i = 0; i < _colorFaces.Length; i++)
                {
                    if (_colorFaces[i] != null) _colorFaces[i].Release();
                    if (_depthFaces[i] != null) _depthFaces[i].Release();
                }

            if (_colorArray != null) _colorArray.Release();
            if (_depthArray != null) _depthArray.Release();
            if (_splatRT != null) _splatRT.Release();
            if (_splatDepthRT != null) _splatDepthRT.Release();
            if (_cameraCubemapRT != null) _cameraCubemapRT.Release();
        }

        void CreateBuffers()
        {
            int maxSplats = 6 * _faceResolution * _faceResolution;
            _splatBuffer = new ComputeBuffer(maxSplats, 32, ComputeBufferType.Append);
            _argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            _argsBuffer.SetData(new uint[] { 6, 0, 0, 0, 0 });
        }

        void ReleaseBuffers()
        {
            _splatBuffer?.Release();
            _argsBuffer?.Release();
        }

        void CreateQuad()
        {
            _quadMesh = new Mesh
            {
                name = "SplatQuad",
                vertices = new[]
                {
                    new Vector3(-1, -1, 0),
                    new Vector3( 1, -1, 0),
                    new Vector3(-1,  1, 0),
                    new Vector3( 1,  1, 0)
                },
                triangles = new[] { 0, 2, 1, 2, 3, 1 }
            };
            _quadMesh.UploadMeshData(true);
        }

        void EnsureSplatRTSize()
        {
            int w = _mainCamera.pixelWidth;
            int h = _mainCamera.pixelHeight;
            if (w <= 0 || h <= 0) return;

            if (_splatRT != null && _splatRT.width == w && _splatRT.height == h)
                return;

            if (_splatRT != null) _splatRT.Release();
            if (_splatDepthRT != null) _splatDepthRT.Release();

            _splatRT = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
            _splatRT.Create();

            _splatDepthRT = new RenderTexture(w, h, 24, RenderTextureFormat.Depth);
            _splatDepthRT.Create();
        }

        void CreateFaceCameras()
        {
            _faceCameras = new Camera[6];

            for (int f = 0; f < 6; f++)
            {
                var go = new GameObject($"Probe_Face{f}")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                var cam = go.AddComponent<Camera>();
                cam.fieldOfView = 90f;
                cam.aspect = 1f;
                cam.nearClipPlane = _mainCamera.nearClipPlane;
                cam.farClipPlane = _mainCamera.farClipPlane;
                cam.targetTexture = _colorFaces[f];
                cam.depth = -1000 + f;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.black;
                cam.enabled = true;

                var urpData = go.AddComponent<UniversalAdditionalCameraData>();
                urpData.requiresDepthTexture = true;
                urpData.renderPostProcessing = false;
                urpData.renderShadows = false;

                _faceCameras[f] = cam;
                _faceCameraLookup[cam] = f;
            }
        }

        void DestroyFaceCameras()
        {
            if (_faceCameras == null) return;
            for (int i = 0; i < _faceCameras.Length; i++)
            {
                if (_faceCameras[i] != null)
                    DestroyImmediate(_faceCameras[i].gameObject);
            }
            _faceCameras = null;
            _faceCameraLookup.Clear();
        }

        void CreateCubemapCamera()
        {
            var go = new GameObject("CubemapCamera")
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            _cubemapCamera = go.AddComponent<Camera>();
            _cubemapCamera.nearClipPlane = _mainCamera.nearClipPlane;
            _cubemapCamera.farClipPlane = _mainCamera.farClipPlane;
            _cubemapCamera.clearFlags = CameraClearFlags.Skybox;
            _cubemapCamera.enabled = false;

            var urpData = go.AddComponent<UniversalAdditionalCameraData>();
            urpData.renderPostProcessing = false;
            urpData.renderShadows = false;
        }

        void DestroyCubemapCamera()
        {
            if (_cubemapCamera != null)
                DestroyImmediate(_cubemapCamera.gameObject);
            _cubemapCamera = null;
        }

        void RenderCameraCubemap()
        {
            _cubemapCamera.transform.position = _mainCamera.transform.position;
            _cubemapCamera.RenderToCubemap(_cameraCubemapRT);
        }

        void PositionProbe()
        {
            Vector3 camPos = _mainCamera.transform.position;
            Vector3 camFwd = _mainCamera.transform.forward;
            float s = _probeGridSpacing;

            Vector3 gridCenter = new Vector3(
                Mathf.Round(camPos.x / s) * s,
                Mathf.Round(camPos.y / s) * s,
                Mathf.Round(camPos.z / s) * s
            );

            bool gridChanged = gridCenter != _previousGridCenter;
            _previousGridCenter = gridCenter;

            if (gridChanged)
                _renderedFaceMask = 0;

            _activeFaceMask = 0;
            for (int f = 0; f < 6; f++)
            {
                if (Vector3.Dot(FaceForward[f], camFwd) > -0.3f)
                    _activeFaceMask |= 1u << f;
            }

            uint needsRenderMask = _activeFaceMask & ~_renderedFaceMask;
            _renderedFaceMask |= needsRenderMask;
            _freshlyRenderedMask = needsRenderMask;

            for (int f = 0; f < 6; f++)
            {
                _faceCameras[f].enabled = (needsRenderMask & (1u << f)) != 0;
                _faceCameras[f].transform.SetPositionAndRotation(
                    gridCenter, Quaternion.LookRotation(FaceForward[f], FaceUp[f]));
            }
        }

        public bool TryGetFaceIndex(Camera cam, out int faceIndex)
            => _faceCameraLookup.TryGetValue(cam, out faceIndex);

        public RenderTexture GetProbeDepthFace(int faceIndex)
        {
            if (_depthFaces == null || faceIndex < 0 || faceIndex >= 6) return null;
            return _depthFaces[faceIndex];
        }

        public RenderTexture GetSplatRT() => _splatRT;
        public RenderTexture GetSplatDepthRT() => _splatDepthRT;

        public void DispatchSplatCompute(CommandBuffer cmd)
        {
            // Only copy faces that were freshly rendered this frame
            for (int f = 0; f < 6; f++)
            {
                if ((_freshlyRenderedMask & (1u << f)) != 0)
                {
                    cmd.CopyTexture(_colorFaces[f], 0, 0, _colorArray, f, 0);
                    cmd.CopyTexture(_depthFaces[f], 0, 0, _depthArray, f, 0);
                }
                _faceInvViewArray[f] = _faceCameras[f].cameraToWorldMatrix;
            }

            // Build active face index list
            int activeFaceCount = 0;
            Vector4 faceIndices0 = Vector4.zero;
            Vector4 faceIndices1 = Vector4.zero;
            for (int f = 0; f < 6; f++)
            {
                if ((_activeFaceMask & (1u << f)) != 0)
                {
                    if (activeFaceCount < 4)
                        faceIndices0[activeFaceCount] = f;
                    else
                        faceIndices1[activeFaceCount - 4] = f;
                    activeFaceCount++;
                }
            }

            if (activeFaceCount == 0) return;

            cmd.SetBufferCounterValue(_splatBuffer, 0);

            cmd.SetComputeFloatParam(_splatCompute, ShaderIDs.FaceResolution, _faceResolution);
            cmd.SetComputeFloatParam(_splatCompute, ShaderIDs.FarPlane, _mainCamera.farClipPlane);
            cmd.SetComputeFloatParam(_splatCompute, ShaderIDs.PosterizeLevels, _posterizeLevels);
            cmd.SetComputeFloatParam(_splatCompute, ShaderIDs.SplatSizeMultiplier, _splatSizeMultiplier);
            cmd.SetComputeVectorParam(_splatCompute, ShaderIDs.FaceIndices0, faceIndices0);
            cmd.SetComputeVectorParam(_splatCompute, ShaderIDs.FaceIndices1, faceIndices1);
            cmd.SetComputeBufferParam(_splatCompute, _computeKernel, ShaderIDs.SplatBuffer, _splatBuffer);

            GeometryUtility.CalculateFrustumPlanes(_mainCamera, _frustumPlanesCache);
            for (int i = 0; i < 6; i++)
            {
                var p = _frustumPlanesCache[i];
                cmd.SetComputeVectorParam(_splatCompute, ShaderIDs.FrustumPlanes[i],
                    new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance));
            }

            cmd.SetComputeTextureParam(_splatCompute, _computeKernel, ShaderIDs.ProbeColorArray, _colorArray);
            cmd.SetComputeTextureParam(_splatCompute, _computeKernel, ShaderIDs.ProbeDepthArray, _depthArray);
            cmd.SetComputeMatrixArrayParam(_splatCompute, ShaderIDs.FaceInvViews, _faceInvViewArray);

            int groups = Mathf.CeilToInt(_faceResolution / 8.0f);
            cmd.DispatchCompute(_splatCompute, _computeKernel, groups, groups, activeFaceCount);

            cmd.CopyCounterValue(_splatBuffer, _argsBuffer, 4);
        }

        public void DrawSplats(CommandBuffer cmd)
        {
            cmd.SetGlobalBuffer(ShaderIDs.SplatBuffer, _splatBuffer);
            cmd.DrawMeshInstancedIndirect(_quadMesh, 0, _splatMaterial, 0, _argsBuffer);
        }

        public void SetCompositeUniforms(CommandBuffer cmd, Camera cam)
        {
            Matrix4x4 vp = cam.projectionMatrix * cam.worldToCameraMatrix;
            cmd.SetGlobalMatrix(ShaderIDs.InvViewProjMatrix, vp.inverse);
            cmd.SetGlobalFloat(ShaderIDs.PosterizeLevels, _posterizeLevels);
            cmd.SetGlobalVector(ShaderIDs.CameraWorldPos, cam.transform.position);
            cmd.SetGlobalTexture(ShaderIDs.CameraCubemap, _cameraCubemapRT);
        }
    }
}
