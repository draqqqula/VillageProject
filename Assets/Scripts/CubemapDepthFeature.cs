using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CubemapDepthFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material depthMaterial;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public Settings settings = new Settings();

    class DepthPass : ScriptableRenderPass
    {
        private Material depthMaterial;
        private FilteringSettings filtering;

        public DepthPass(Material mat, RenderPassEvent evt)
        {
            depthMaterial = mat;
            renderPassEvent = evt;

            filtering = new FilteringSettings(RenderQueueRange.opaque);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (depthMaterial == null)
                return;

            var cmd = CommandBufferPool.Get("Cubemap Radial Depth");

            using (new ProfilingScope(cmd, new ProfilingSampler("Cubemap Depth Pass")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawSettings = new DrawingSettings();

                drawSettings.sortingSettings = new SortingSettings(renderingData.cameraData.camera)
                {
                    criteria = SortingCriteria.CommonOpaque
                };

                drawSettings.overrideMaterial = depthMaterial;
                drawSettings.overrideMaterialPassIndex = 0;

                drawSettings.SetShaderPassName(0, new ShaderTagId("UniversalForward"));
                drawSettings.SetShaderPassName(1, new ShaderTagId("UniversalForwardOnly"));
                drawSettings.SetShaderPassName(2, new ShaderTagId("SRPDefaultUnlit"));

                drawSettings.overrideMaterial = depthMaterial;

                context.DrawRenderers(
                    renderingData.cullResults,
                    ref drawSettings,
                    ref filtering
                );
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    DepthPass pass;

    public override void Create()
    {
        pass = new DepthPass(settings.depthMaterial, settings.passEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}