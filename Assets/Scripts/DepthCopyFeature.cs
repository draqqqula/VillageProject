using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthCopyFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Camera targetCamera;
        public Material depthCopyMaterial;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRendering;
    }

    public Settings settings = new Settings();

    DepthCopyPass _pass;

    public override void Create()
    {
        _pass = new DepthCopyPass(
            settings.targetCamera,
            settings.depthCopyMaterial
        );

        _pass.renderPassEvent = settings.passEvent;
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (settings.depthCopyMaterial == null)
            return;

        _pass.Setup();
        renderer.EnqueuePass(_pass);
    }
}