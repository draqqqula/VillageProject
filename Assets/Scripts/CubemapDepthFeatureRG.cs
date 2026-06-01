using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CubemapDepthFeatureRG : ScriptableRendererFeature
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

        private readonly FilteringSettings filtering =
            new FilteringSettings(RenderQueueRange.all);

        private static readonly List<ShaderTagId> shaderTags =
            new List<ShaderTagId>
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("SRPDefaultUnlit")
            };

        private class PassData
        {
            public RendererListHandle rendererList;
        }

        public void Setup(Material mat, RenderPassEvent evt)
        {
            depthMaterial = mat;
            renderPassEvent = evt;
        }

        public override void RecordRenderGraph(
            RenderGraph renderGraph,
            ContextContainer frameData)
        {
            if (depthMaterial == null)
                return;

            var renderingData = frameData.Get<UniversalRenderingData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var lightData = frameData.Get<UniversalLightData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            if (!resourceData.activeColorTexture.IsValid())
                return;

            // Если тут invalid — почти наверняка у Camera Output RenderTexture
            // Depth Stencil Format стоит None.
            if (!resourceData.activeDepthTexture.IsValid())
                return;

            var drawingSettings = RenderingUtils.CreateDrawingSettings(
                shaderTags,
                renderingData,
                cameraData,
                lightData,
                SortingCriteria.CommonOpaque
            );

            drawingSettings.overrideMaterial = depthMaterial;
            drawingSettings.overrideMaterialPassIndex = 0;

            var rendererListParams = new RendererListParams(
                renderingData.cullResults,
                drawingSettings,
                filtering
            );

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(
                "Cubemap Radial Depth",
                out var passData))
            {
                passData.rendererList =
                    renderGraph.CreateRendererList(rendererListParams);

                builder.UseRendererList(passData.rendererList);

                // Оставляем активный color target камеры.
                builder.SetRenderAttachment(
                    resourceData.activeColorTexture,
                    0,
                    AccessFlags.ReadWrite
                );

                // ReadWrite, потому что pass после opaque может зависеть
                // от уже существующего depth.
                builder.SetRenderAttachmentDepth(
                    resourceData.activeDepthTexture,
                    AccessFlags.ReadWrite
                );

                builder.AllowPassCulling(false);

                builder.SetRenderFunc(static (PassData data, RasterGraphContext ctx) =>
                {
                    ctx.cmd.DrawRendererList(data.rendererList);
                });
            }
        }
    }

    private DepthPass pass;

    public override void Create()
    {
        pass = new DepthPass();
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (settings.depthMaterial == null)
            return;

        pass.Setup(settings.depthMaterial, settings.passEvent);
        renderer.EnqueuePass(pass);
    }
}