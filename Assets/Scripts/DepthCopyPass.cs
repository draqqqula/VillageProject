using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class DepthCopyPass : ScriptableRenderPass
{
    Camera _targetCamera;
    Material _material;

    RenderTexture _destination;

    static readonly int DepthTexID = Shader.PropertyToID("_DepthCopyRT");

    public DepthCopyPass(Camera cam, Material mat)
    {
        _targetCamera = cam;
        _material = mat;
    }

    public void Setup() { }

    public void SetTarget(RenderTexture rt)
    {
        _destination = rt;
    }

    public override void Execute(
        ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        Camera cam = renderingData.cameraData.camera;

        // ❗ ВАЖНО: работаем ТОЛЬКО с нужной камерой
        if (cam != _targetCamera)
            return;

        if (_destination == null)
            return;

        CommandBuffer cmd =
            CommandBufferPool.Get("Copy Camera Depth");

        // null source → depth берётся из _CameraDepthTexture
        cmd.Blit(null, _destination, _material);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}