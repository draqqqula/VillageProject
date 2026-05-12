using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class CubemapRendererEarly : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera captureCamera;
    [SerializeField] private Material material;
    [SerializeField] private Material _depthMaterial;
    [SerializeField] private Vector2Int _cubemapResolution;
    [SerializeField] private bool _renderDepth = true;

    RenderTexture cubemapTexture;
    RenderTexture depthTexture;
    UniversalAdditionalCameraData cameraData;

    void Start()
    {
        cameraData = captureCamera.GetUniversalAdditionalCameraData();

        cubemapTexture = new RenderTexture(_cubemapResolution.x, _cubemapResolution.y, 16);
        cubemapTexture.dimension = TextureDimension.Cube;
        cubemapTexture.useMipMap = false;
        cubemapTexture.autoGenerateMips = false;
        cubemapTexture.filterMode = FilterMode.Point;
        cubemapTexture.Create();

        if (_renderDepth)
        {
            depthTexture = new RenderTexture(_cubemapResolution.x, _cubemapResolution.y, 16);
            depthTexture.dimension = TextureDimension.Cube;
            depthTexture.useMipMap = false;
            depthTexture.autoGenerateMips = false;
            depthTexture.filterMode = FilterMode.Point;
            depthTexture.Create();
        }
        material.SetTexture("_Cube", cubemapTexture);
        material.SetTexture("_DepthCube", depthTexture);
    }

    void LateUpdate()
    {
        captureCamera.transform.position = mainCamera.transform.position;
        
        if (_renderDepth)
        {
            captureCamera.backgroundColor = Color.magenta;
            cameraData.SetRenderer(0);
            captureCamera.RenderToCubemap(cubemapTexture);

            _depthMaterial.SetVector("_CubemapOrigin", captureCamera.transform.position);
            _depthMaterial.SetFloat("_DepthNear", captureCamera.nearClipPlane);
            _depthMaterial.SetFloat("_DepthFar", captureCamera.farClipPlane);

            captureCamera.backgroundColor = Color.red;
            cameraData.SetRenderer(3);
            captureCamera.RenderToCubemap(depthTexture);
        }
        else
        {
            captureCamera.RenderToCubemap(cubemapTexture);
        }
    }
}