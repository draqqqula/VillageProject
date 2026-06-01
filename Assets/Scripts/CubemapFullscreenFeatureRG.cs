using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class CubemapFullscreenFeatureRG : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;

        // Как у встроенного Full Screen Pass Renderer Feature.
        // Не ставь AfterRendering для fullscreen эффекта.
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPostProcessing;

        public int passIndex = 0;

        // true — если shader читает текущую картинку через _BlitTexture.
        // false — если shader просто рисует поверх/вместо картинки и не читает screen color.
        public bool fetchColorBuffer = true;

        // Включай только если fullscreen shader реально использует depth/stencil state.
        public bool bindDepthStencilAttachment = false;

        // Обычно None.
        // Если shader хочет _CameraDepthTexture / normals / motion — добавь нужное.
        public ScriptableRenderPassInput requirements = ScriptableRenderPassInput.None;
    }

    public Settings settings = new Settings();

    class FullscreenPass : ScriptableRenderPass
    {
        private static readonly int BlitTextureId =
            Shader.PropertyToID("_BlitTexture");

        private static readonly int BlitScaleBiasId =
            Shader.PropertyToID("_BlitScaleBias");

        private readonly MaterialPropertyBlock propertyBlock =
            new MaterialPropertyBlock();

        private Material material;
        private int passIndex;
        private bool fetchColorBuffer;
        private bool bindDepthStencilAttachment;

        private class PassData
        {
            public Material material;
            public MaterialPropertyBlock propertyBlock;
            public int passIndex;
            public TextureHandle source;
            public TextureHandle destination;
            public bool hasSource;
        }

        public void Setup(
            Material material,
            int passIndex,
            bool fetchColorBuffer,
            bool bindDepthStencilAttachment,
            RenderPassEvent passEvent,
            ScriptableRenderPassInput requirements)
        {
            this.material = material;
            this.passIndex = passIndex;
            this.fetchColorBuffer = fetchColorBuffer;
            this.bindDepthStencilAttachment = bindDepthStencilAttachment;

            renderPassEvent = passEvent;

            ConfigureInput(requirements);

            // ВАЖНО:
            // если материал читает текущий color buffer,
            // URP должен создать intermediate color texture.
            requiresIntermediateTexture = fetchColorBuffer;
        }

        public override void RecordRenderGraph(
            RenderGraph renderGraph,
            ContextContainer frameData)
        {
            if (material == null)
                return;

            if (passIndex < 0 || passIndex >= material.passCount)
                return;

            var resourceData = frameData.Get<UniversalResourceData>();

            TextureHandle source;

            if (fetchColorBuffer)
            {
                // Именно cameraColor, не activeColorTexture.
                // activeColorTexture может быть backbuffer.
                if (!resourceData.cameraColor.IsValid())
                    return;

                var desc = renderGraph.GetTextureDesc(resourceData.cameraColor);
                desc.name = "_CubemapFullscreenColorCopy";
                desc.clearBuffer = false;

                TextureHandle colorCopy = renderGraph.CreateTexture(desc);

                // Копируем текущий camera color в отдельную texture,
                // чтобы shader мог читать её как _BlitTexture.
                renderGraph.AddBlitPass(
                    resourceData.cameraColor,
                    colorCopy,
                    Vector2.one,
                    Vector2.zero,
                    passName: "Cubemap Fullscreen Copy Color"
                );

                source = colorCopy;
            }
            else
            {
                source = TextureHandle.nullHandle;
            }

            TextureHandle destination = resourceData.activeColorTexture;

            if (!destination.IsValid())
                return;

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(
                "Cubemap Fullscreen Material Pass",
                out var passData))
            {
                passData.material = material;
                passData.propertyBlock = propertyBlock;
                passData.passIndex = passIndex;
                passData.source = source;
                passData.destination = destination;
                passData.hasSource = source.IsValid();

                if (passData.hasSource)
                    builder.UseTexture(source, AccessFlags.Read);

                builder.SetRenderAttachment(
                    destination,
                    0,
                    AccessFlags.Write
                );

                if (bindDepthStencilAttachment &&
                    resourceData.activeDepthTexture.IsValid())
                {
                    builder.SetRenderAttachmentDepth(
                        resourceData.activeDepthTexture,
                        AccessFlags.ReadWrite
                    );
                }

                builder.AllowPassCulling(false);

                builder.SetRenderFunc(static (PassData data, RasterGraphContext ctx) =>
                {
                    data.propertyBlock.Clear();

                    if (data.hasSource)
                    {
                        data.propertyBlock.SetTexture(
                            BlitTextureId,
                            data.source
                        );
                    }

                    ctx.cmd.DrawProcedural(
                        Matrix4x4.identity,
                        data.material,
                        data.passIndex,
                        MeshTopology.Triangles,
                        3,
                        1,
                        data.propertyBlock
                    );
                });
            }
        }
    }

    private FullscreenPass pass;

    public override void Create()
    {
        pass = new FullscreenPass();
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        pass.Setup(
            settings.material,
            settings.passIndex,
            settings.fetchColorBuffer,
            settings.bindDepthStencilAttachment,
            settings.passEvent,
            settings.requirements
        );

        renderer.EnqueuePass(pass);
    }
}