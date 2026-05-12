using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CubeMapRenderPass : ScriptableRenderPass
{
    int cubeResolution;
    Material cubeMaterial;
    RenderTargetIdentifier colorTarget;

    RenderTexture cubeRT;
    static readonly string profilerTag = "CubeMap Render Pass";

    public CubeMapRenderPass(int resolution, Material material)
    {
        cubeResolution = resolution;
        cubeMaterial = material;
    }

    public void Setup(RenderTargetIdentifier colorTarget)
    {
        this.colorTarget = colorTarget;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Создаём кубмапу
        if (cubeRT == null || cubeRT.width != cubeResolution)
        {
            if (cubeRT != null) cubeRT.Release();
            cubeRT = new RenderTexture(cubeResolution, cubeResolution, 16);
            cubeRT.dimension = TextureDimension.Cube;
            cubeRT.enableRandomWrite = false;
            cubeRT.useMipMap = false;
            cubeRT.autoGenerateMips = false;
            cubeRT.Create();
        }
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get(profilerTag);

        Camera cam = renderingData.cameraData.camera;
        Vector3 camPos = cam.transform.position;
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        // Рендерим 6 граней куба вручную
        for (int face = 0; face < 6; face++)
        {
            Matrix4x4 view = GetCubeViewMatrix((CubemapFace)face, camPos);
            Matrix4x4 proj = Matrix4x4.Perspective(90f, 1f, near, far);

            cmd.SetRenderTarget(cubeRT, 0, (CubemapFace)face);
            cmd.ClearRenderTarget(true, true, Color.black);
            cmd.SetViewProjectionMatrices(view, proj);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            // Выполняем culling для текущей камеры
            ScriptableCullingParameters cullingParams;
            if (!cam.TryGetCullingParameters(out cullingParams)) continue;
            CullingResults cullResults = context.Cull(ref cullingParams);

            // Рендерим все объекты сцены
            var drawSettings = CreateDrawingSettings(
                new ShaderTagId("UniversalForward"),
                ref renderingData,
                SortingCriteria.CommonOpaque
            );
            var filterSettings = new FilteringSettings(RenderQueueRange.all);

            context.DrawRenderers(cullResults, ref drawSettings, ref filterSettings);

            // Skybox
            context.DrawSkybox(cam);
        }

        // После кубмапы делаем fullscreen pass с обработкой
        cmd.SetGlobalTexture("_Cube", cubeRT);
        //Blit(cmd, colorTarget, colorTarget, cubeMaterial);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        // Если нужно — можно освобождать ресурсы
    }

    // Вспомогательная функция для view матриц куба
    private Matrix4x4 GetCubeViewMatrix(CubemapFace face, Vector3 pos)
    {
        switch (face)
        {
            case CubemapFace.PositiveX: return Matrix4x4.LookAt(pos, pos + Vector3.right, Vector3.up);
            case CubemapFace.NegativeX: return Matrix4x4.LookAt(pos, pos + Vector3.left, Vector3.up);
            case CubemapFace.PositiveY: return Matrix4x4.LookAt(pos, pos + Vector3.up, Vector3.forward);
            case CubemapFace.NegativeY: return Matrix4x4.LookAt(pos, pos + Vector3.down, Vector3.back);
            case CubemapFace.PositiveZ: return Matrix4x4.LookAt(pos, pos + Vector3.forward, Vector3.up);
            case CubemapFace.NegativeZ: return Matrix4x4.LookAt(pos, pos + Vector3.back, Vector3.up);
        }
        return Matrix4x4.identity;
    }
}