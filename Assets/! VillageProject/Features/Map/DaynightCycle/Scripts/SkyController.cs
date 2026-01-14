using UnityEngine;

public class SkyController : MonoBehaviour
{
    [SerializeField] private Material _runtimeSkyboxMaterial;

    private static readonly int SunDiscColor = Shader.PropertyToID("_SunDiscColor");
    private static readonly int SunDiscMultiplier = Shader.PropertyToID("_SunDiscMultiplier");
    private static readonly int SunDiscExponent = Shader.PropertyToID("_SunDiscExponent");

    private static readonly int SunHaloColor = Shader.PropertyToID("_SunHaloColor");
    private static readonly int SunHaloExponent = Shader.PropertyToID("_SunHaloExponent");
    private static readonly int SunHaloContribution = Shader.PropertyToID("_SunHaloContribution");

    private static readonly int HorizonColor = Shader.PropertyToID("_HorizonLineColor");
    private static readonly int HorizonExponent = Shader.PropertyToID("_HorizonLineExponent");
    private static readonly int HorizonContribution = Shader.PropertyToID("_HorizonLineContribution");

    private static readonly int SkyTop = Shader.PropertyToID("_SkyGradientTop");
    private static readonly int SkyBottom = Shader.PropertyToID("_SkyGradientBottom");
    private static readonly int SkyGradientExponent = Shader.PropertyToID("_SkyGradientExponent");

    private void Start()
    {
        _runtimeSkyboxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = _runtimeSkyboxMaterial;
    }

    #region Sun Disc

    public void SetSunDiscColor(Color color)
    {
        _runtimeSkyboxMaterial.SetColor(SunDiscColor, color);
    }

    public void SetSunDiscMultiplier(float value)
    {
        _runtimeSkyboxMaterial.SetFloat(SunDiscMultiplier, value);
    }

    public void SetSunDiscExponent(float value)
    {
        _runtimeSkyboxMaterial.SetFloat(SunDiscExponent, value);
    }

    #endregion

    #region Sun Halo

    public void SetSunHaloColor(Color color)
    {
        _runtimeSkyboxMaterial.SetColor(SunHaloColor, color);
    }

    public void SetSunHaloExponent(float value)
    {
        _runtimeSkyboxMaterial.SetFloat(SunHaloExponent, value);
    }

    public void SetSunHaloContribution(float value)
    {
        _runtimeSkyboxMaterial.SetFloat(SunHaloContribution, value);
    }

    #endregion

    #region Horizon Line

    public void SetHorizonColor(Color color)
    {
        _runtimeSkyboxMaterial.SetColor(HorizonColor, color);
    }

    public void SetHorizonExponent(float value)
    {
        _runtimeSkyboxMaterial.SetFloat(HorizonExponent, value);
    }

    public void SetHorizonContribution(float value)
    {
        _runtimeSkyboxMaterial.SetFloat(HorizonContribution, value);
    }

    #endregion

    #region Sky Gradient

    public void SetSkyTopColor(Color color)
    {
        _runtimeSkyboxMaterial.SetColor(SkyTop, color);
    }

    public void SetSkyBottomColor(Color color)
    {
        _runtimeSkyboxMaterial.SetColor(SkyBottom, color);
    }

    public void SetSkyGradientExponent(float value)
    {
        _runtimeSkyboxMaterial.SetFloat(SkyGradientExponent, value);
    }

    #endregion
}