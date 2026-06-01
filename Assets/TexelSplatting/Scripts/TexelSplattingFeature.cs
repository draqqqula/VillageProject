using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace TexelSplatting
{
    public class TexelSplattingFeature : ScriptableRendererFeature
    {
        ProbeDepthCapturePass _depthCapturePass;
        SplatCompositePass _compositePass;

        public override void Create()
        {
            _depthCapturePass = new ProbeDepthCapturePass
            {
                renderPassEvent = RenderPassEvent.AfterRendering
            };

            _compositePass = new SplatCompositePass
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var controller = TexelSplattingController.Instance;
            if (controller == null)
                return;

            var camera = renderingData.cameraData.camera;

            if (controller.TryGetFaceIndex(camera, out _))
                renderer.EnqueuePass(_depthCapturePass);
            else if (camera == controller.MainCamera)
                renderer.EnqueuePass(_compositePass);
        }

        class DepthPassData
        {
            internal TextureHandle depthTexture;
            internal RenderTexture depthTarget;
            internal Material material;
        }

        class ProbeDepthCapturePass : ScriptableRenderPass
        {
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var controller = TexelSplattingController.Instance;
                if (controller == null) return;

                var cameraData = frameData.Get<UniversalCameraData>();
                if (!controller.TryGetFaceIndex(cameraData.camera, out int faceIndex)) return;

                var depthTarget = controller.GetProbeDepthFace(faceIndex);
                if (depthTarget == null) return;

                using (var builder = renderGraph.AddUnsafePass<DepthPassData>("TexelSplat_DepthCapture", out var passData))
                {
                    var resourceData = frameData.Get<UniversalResourceData>();
                    passData.depthTexture = resourceData.cameraDepthTexture;
                    passData.depthTarget = depthTarget;
                    passData.material = controller.LinearizeDepthMat;

                    builder.UseTexture(passData.depthTexture, AccessFlags.Read);
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc(static (DepthPassData data, UnsafeGraphContext context) =>
                    {
                        var cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                        cmd.Blit(Shader.PropertyToID("_CameraDepthTexture"), data.depthTarget, data.material);
                    });
                }
            }
        }

        class CompositePassData
        {
            internal TexelSplattingController controller;
            internal Camera cam;
            internal TextureHandle cameraColor;
        }

        class SplatCompositePass : ScriptableRenderPass
        {
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var controller = TexelSplattingController.Instance;
                if (controller == null) return;

                var cameraData = frameData.Get<UniversalCameraData>();
                var resourceData = frameData.Get<UniversalResourceData>();

                using (var builder = renderGraph.AddUnsafePass<CompositePassData>("TexelSplat_Composite", out var passData))
                {
                    passData.controller = controller;
                    passData.cam = cameraData.camera;
                    passData.cameraColor = resourceData.activeColorTexture;

                    builder.UseTexture(passData.cameraColor, AccessFlags.WriteAll);
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc(static (CompositePassData data, UnsafeGraphContext context) =>
                    {
                        var cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

                        data.controller.DispatchSplatCompute(cmd);

                        var splatRT = data.controller.GetSplatRT();
                        var splatDepthRT = data.controller.GetSplatDepthRT();
                        cmd.SetRenderTarget(splatRT, splatDepthRT);
                        cmd.ClearRenderTarget(true, true, Color.clear);
                        cmd.SetViewProjectionMatrices(data.cam.worldToCameraMatrix, data.cam.projectionMatrix);
                        data.controller.DrawSplats(cmd);

                        data.controller.SetCompositeUniforms(cmd, data.cam);
                        context.cmd.SetRenderTarget(data.cameraColor);
                        cmd.Blit(splatRT, BuiltinRenderTextureType.CurrentActive, data.controller.CompositeMat);
                    });
                }
            }
        }
    }
}
