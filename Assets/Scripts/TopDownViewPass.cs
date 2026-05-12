using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class TopDownViewPass : ScriptableRenderPass
{
    private const string PASS_NAME = "TopDown View Pass";

    public TopDownViewPass(TopDownViewFeature.Settings settings)
    {
        renderPassEvent = settings.renderPassEvent;
        requiresIntermediateTexture = true;
    }

    class PassData
    {
        public RendererListHandle rendererList;
        public TextureHandle target;
        public Matrix4x4 view;
        public Matrix4x4 proj;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var cameraData = frameData.Get<UniversalCameraData>();
        var renderingData = frameData.Get<UniversalRenderingData>();
        var resourceData = frameData.Get<UniversalResourceData>();

        if (resourceData.isActiveTargetBackBuffer)
            return;

        Camera cam = cameraData.camera;

        // --- TOP DOWN VIEW ---
        Vector3 pos = cam.transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        Matrix4x4 view = Matrix4x4.TRS(pos, rot, Vector3.one).inverse;
        Matrix4x4 proj = cameraData.GetProjectionMatrix();

        // --- TARGET TEXTURE ---
        var desc = renderGraph.GetTextureDesc(resourceData.cameraColor);
        desc.name = "TopDownColor";
        desc.clearBuffer = true;

        TextureHandle topDownTexture = renderGraph.CreateTexture(desc);

        // --- RendererList (НОВЫЙ API) ---
        var rendererListDesc = new RendererListDesc(
            new ShaderTagId("UniversalForward"),
            renderingData.cullResults,
            cam
        )
        {
            renderQueueRange = RenderQueueRange.all,
            sortingCriteria = SortingCriteria.CommonOpaque
        };

        var rendererList = renderGraph.CreateRendererList(rendererListDesc);

        // --- PASS ---
        using (var builder = renderGraph.AddRasterRenderPass<PassData>(PASS_NAME, out var passData))
        {
            passData.rendererList = rendererList;
            passData.target = topDownTexture;
            passData.view = view;
            passData.proj = proj;

            builder.UseRendererList(passData.rendererList);
            builder.SetRenderAttachment(passData.target, 0);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                ctx.cmd.SetViewProjectionMatrices(data.view, data.proj);
                ctx.cmd.DrawRendererList(data.rendererList);
            });
        }

        // --- BLIT BACK ---
        var blit = new RenderGraphUtils.BlitMaterialParameters(
            topDownTexture,
            resourceData.cameraColor,
            Blitter.GetBlitMaterial(TextureDimension.Tex2D),
            0
        );

        renderGraph.AddBlitPass(blit, PASS_NAME + " Blit");
    }
}