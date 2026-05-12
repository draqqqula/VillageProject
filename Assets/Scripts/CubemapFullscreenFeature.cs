using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CubemapFullscreenFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRendering;
    }

    public Settings settings = new Settings();

    class Pass : ScriptableRenderPass
    {
        Material material;

        RTHandle source;
        RTHandle temp;

        public Pass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.AfterRendering + 1;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(
                ref temp,
                desc,
                name: "_TempFullscreenRT"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Fullscreen Pass Fix");

            // 1. source → temp
            Blitter.BlitCameraTexture(cmd, source, temp, material, 0);

            // 2. temp → source
            Blitter.BlitCameraTexture(cmd, temp, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            temp?.Release();
        }
    }

    Pass pass;

    public override void Create()
    {
        pass = new Pass(settings.material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}