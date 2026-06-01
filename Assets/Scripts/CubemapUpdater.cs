using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class CubemapUpdater : MonoBehaviour
{
    public Camera captureCamera;
    public Material targetMaterial;

    public int resolution = 512;
    public bool updateEveryFrame = true;

    private RenderTexture cubeRT;

    void Start()
    {
        cubeRT = new RenderTexture(resolution, resolution, 16);
        cubeRT.dimension = TextureDimension.Cube;
        cubeRT.useMipMap = true;
        cubeRT.autoGenerateMips = true;
        cubeRT.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

        cubeRT.Create();

        if (targetMaterial != null)
        {
            targetMaterial.SetTexture("_Cube", cubeRT);
        }
    }

    void LateUpdate()
    {
        if (!updateEveryFrame || captureCamera == null)
            return;

        // синхронизируем позицию с объектом (обычно с игроком)
        captureCamera.transform.position = transform.position;

        // рендерим cubemap
        captureCamera.RenderToCubemap(cubeRT);

        targetMaterial.SetTexture("_Cube", cubeRT);
    }

    void OnDestroy()
    {
        if (cubeRT != null)
        {
            cubeRT.Release();
        }
    }
}