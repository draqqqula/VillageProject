using UnityEngine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class RealtimeCubemapFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        public int cubemapResolution = 256;
        public Material cubemapMaterial; // Материал для обработки куба
    }

    public CustomPassSettings settings = new CustomPassSettings();
    private RealtimeCubemapPass cubemapPass;

    public override void Create()
    {
        cubemapPass = new RealtimeCubemapPass(settings);
        cubemapPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(cubemapPass);
    }

    // --------------------- PASS ---------------------
    public class RealtimeCubemapPass : ScriptableRenderPass
    {
        private const string PASS_NAME = "Realtime Cubemap Pass";
        private CustomPassSettings settings;

        public RealtimeCubemapPass(CustomPassSettings settings)
        {
            this.settings = settings;
            requiresIntermediateTexture = true; // Для RenderGraph
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogWarning("CubemapPass skipped: requires intermediate texture.");
                return;
            }

            Vector3 camPos = cameraData.camera.transform.position;

            // -------------------- Создание TextureHandle для кубмапы --------------------
            var cubeDesc = renderGraph.GetTextureDesc(resourceData.cameraColor);
            cubeDesc.dimension = TextureDimension.Cube;
            cubeDesc.width = settings.cubemapResolution;
            cubeDesc.height = settings.cubemapResolution;
            cubeDesc.name = "RealtimeCubemap";
            cubeDesc.clearBuffer = true;

            TextureHandle cubemapHandle = renderGraph.CreateTexture(cubeDesc);

            // -------------------- Рендерим 6 граней --------------------
            for (int face = 0; face < 6; face++)
            {
                var faceDesc = renderGraph.GetTextureDesc(resourceData.cameraColor);
                faceDesc.dimension = TextureDimension.Tex2D;
                faceDesc.width = settings.cubemapResolution;
                faceDesc.height = settings.cubemapResolution;
                faceDesc.name = $"CubemapFace_{face}";
                faceDesc.clearBuffer = true;

                TextureHandle faceTex = renderGraph.CreateTexture(faceDesc);

                // Blit с материалом (Pass 0 — обработка куба)
                var blitParams = new RenderGraphUtils.BlitMaterialParameters(
                    resourceData.cameraColor, // исходная текстура
                    faceTex,
                    settings.cubemapMaterial,
                    0
                );
                renderGraph.AddBlitPass(blitParams, $"{PASS_NAME} Face {face}");
            }

            // -------------------- Fullscreen pass: взгляд изнутри --------------------
            var finalDesc = renderGraph.GetTextureDesc(resourceData.cameraColor);
            finalDesc.name = "CubemapFinal";
            finalDesc.clearBuffer = false;

            TextureHandle finalBuffer = renderGraph.CreateTexture(finalDesc);

            var blitBack = new RenderGraphUtils.BlitMaterialParameters(
                cubemapHandle,           // теперь кубмапа в виде TextureHandle
                finalBuffer,
                settings.cubemapMaterial,
                1 // Pass #1 — Fullscreen взгляд изнутри
            );
            renderGraph.AddBlitPass(blitBack, PASS_NAME + " Final Blit");

            resourceData.cameraColor = finalBuffer;
        }

        private Matrix4x4 GetCubeViewMatrix(CubemapFace face, Vector3 pos)
        {
            switch (face)
            {
                case CubemapFace.PositiveX: return Matrix4x4.LookAt(pos, pos + Vector3.right, Vector3.up);
                case CubemapFace.NegativeX: return Matrix4x4.LookAt(pos, pos + Vector3.left, Vector3.up);
                case CubemapFace.PositiveY: return Matrix4x4.LookAt(pos, pos + Vector3.up, Vector3.back);
                case CubemapFace.NegativeY: return Matrix4x4.LookAt(pos, pos + Vector3.down, Vector3.forward);
                case CubemapFace.PositiveZ: return Matrix4x4.LookAt(pos, pos + Vector3.forward, Vector3.up);
                case CubemapFace.NegativeZ: return Matrix4x4.LookAt(pos, pos + Vector3.back, Vector3.up);
            }
            return Matrix4x4.identity;
        }
    }
}