using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class NewPixelizePass : ScriptableRenderPass
{
    private const string PASS_NAME = "Pixelize Pass";

    private PixelizeFeature.CustomPassSettings settings;
    private Material material;

    private int pixelScreenHeight;
    private int pixelScreenWidth;

    public NewPixelizePass(PixelizeFeature.CustomPassSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;

        if (material == null)
            material = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");

        // ВАЖНО для RenderGraph
        requiresIntermediateTexture = true;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();

        // Проверка (как в примере)
        if (resourceData.isActiveTargetBackBuffer)
        {
            Debug.LogWarning("PixelizePass skipped: requires intermediate texture.");
            return;
        }

        var source = resourceData.cameraColor;

        // --- ЛОГИКА ПИКСЕЛИЗАЦИИ ---
        pixelScreenHeight = settings.screenHeight;
        pixelScreenWidth = (int)(pixelScreenHeight * cameraData.camera.aspect + 0.5f);

        material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
        material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

        // --- СОЗДАНИЕ ТЕКСТУРЫ ---
        var desc = renderGraph.GetTextureDesc(source);
        desc.width = pixelScreenWidth;
        desc.height = pixelScreenHeight;
        desc.name = "PixelizeBuffer";
        desc.filterMode = FilterMode.Point;
        desc.clearBuffer = false;

        TextureHandle pixelBuffer = renderGraph.CreateTexture(desc);

        // --- ПЕРВЫЙ BLIT (в низкое разрешение с материалом) ---
        var blitToPixel = new RenderGraphUtils.BlitMaterialParameters(
            source,
            pixelBuffer,
            material,
            0
        );
        blitToPixel.sourceTexturePropertyID = Shader.PropertyToID("_MainTex");

        renderGraph.AddBlitPass(blitToPixel, PASS_NAME + " Downsample");

        // --- ВТОРОЙ BLIT (обратно в полный экран) ---
        var descFull = renderGraph.GetTextureDesc(source);
        descFull.name = "PixelizeFinal";
        descFull.clearBuffer = false;

        TextureHandle finalBuffer = renderGraph.CreateTexture(descFull);

        var blitBack = new RenderGraphUtils.BlitMaterialParameters(
            pixelBuffer,
            finalBuffer,
            Blitter.GetBlitMaterial(TextureDimension.Tex2D),
            0
        );

        renderGraph.AddBlitPass(blitBack, PASS_NAME + " Upsample");

        resourceData.cameraColor = finalBuffer;
    }
}