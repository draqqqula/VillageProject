using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public SkyController skyController;

    [Header("Sun Disc")]
    public Gradient sunDiscColor;

    public AnimationCurve sunDiscMultiplier;
    public float sunDiscMultiplierMin;
    public float sunDiscMultiplierMax;

    public AnimationCurve sunDiscExponent;
    public float sunDiscExponentMin;
    public float sunDiscExponentMax;

    [Header("Sun Halo")]
    public Gradient sunHaloColor;

    public AnimationCurve sunHaloExponent;
    public float sunHaloExponentMin;
    public float sunHaloExponentMax;

    public AnimationCurve sunHaloContribution;
    public float sunHaloContributionMin;
    public float sunHaloContributionMax;

    [Header("Horizon Line")]
    public Gradient horizonColor;

    public AnimationCurve horizonExponent;
    public float horizonExponentMin;
    public float horizonExponentMax;

    public AnimationCurve horizonContribution;
    public float horizonContributionMin;
    public float horizonContributionMax;

    [Header("Sky Gradient")]
    public Gradient skyTopColor;
    public Gradient skyBottomColor;

    public AnimationCurve skyGradientExponent;
    public float skyGradientExponentMin;
    public float skyGradientExponentMax;

    public float Time { get; private set; }

    public void UpdateSkybox()
    {
        var t = Time % 1f;

        // Sun Disc
        skyController.SetSunDiscColor(sunDiscColor.Evaluate(t));
        skyController.SetSunDiscMultiplier(
            sunDiscMultiplierMin +
            sunDiscMultiplier.Evaluate(t) *
            (sunDiscMultiplierMax - sunDiscMultiplierMin));

        skyController.SetSunDiscExponent(
            sunDiscExponentMin +
            sunDiscExponent.Evaluate(t) *
            (sunDiscExponentMax - sunDiscExponentMin));

        // Sun Halo
        skyController.SetSunHaloColor(sunHaloColor.Evaluate(t));
        skyController.SetSunHaloExponent(
            sunHaloExponentMin +
            sunHaloExponent.Evaluate(t) *
            (sunHaloExponentMax - sunHaloExponentMin));

        skyController.SetSunHaloContribution(
            sunHaloContributionMin +
            sunHaloContribution.Evaluate(t) *
            (sunHaloContributionMax - sunHaloContributionMin));

        // Horizon Line
        skyController.SetHorizonColor(horizonColor.Evaluate(t));
        skyController.SetHorizonExponent(
            horizonExponentMin +
            horizonExponent.Evaluate(t) *
            (horizonExponentMax - horizonExponentMin));

        skyController.SetHorizonContribution(
            horizonContributionMin +
            horizonContribution.Evaluate(t) *
            (horizonContributionMax - horizonContributionMin));

        // Sky Gradient
        skyController.SetSkyTopColor(skyTopColor.Evaluate(t));
        skyController.SetSkyBottomColor(skyBottomColor.Evaluate(t));
        skyController.SetSkyGradientExponent(
            skyGradientExponentMin +
            skyGradientExponent.Evaluate(t) *
            (skyGradientExponentMax - skyGradientExponentMin));
    }

    public void SetTime(float value)
    {
        var clamped = value % 1f;
        if (Time == clamped)
            return;

        Time = clamped;
        UpdateSkybox();
    }
}