using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SkyboxCycleHelper : MonoBehaviour
{
    [Serializable]
    public class SkyboxAndTime
    {
        public float Time;
        public Material Skybox;
    }

    [SerializeField] private DayNightController dnc;
    [SerializeField] private List<SkyboxAndTime> skyboxes;

#if UNITY_EDITOR
    [ContextMenu("BakeIntoCycle")]
    public void BakeIntoCycle()
    {
        if (dnc == null || skyboxes == null || skyboxes.Count == 0)
        {
            Debug.LogError("SkyboxCycleHelper: invalid setup");
            return;
        }

        skyboxes.Sort((a, b) => a.Time.CompareTo(b.Time));

        // -------- Collect raw values --------
        var discMult = Collect("_SunDiscMultiplier");
        var discExp = Collect("_SunDiscExponent");
        var haloExp = Collect("_SunHaloExponent");
        var haloCon = Collect("_SunHaloContribution");
        var horExp = Collect("_HorizonLineExponent");
        var horCon = Collect("_HorizonLineContribution");
        var skyExp = Collect("_SkyGradientExponent");

        // -------- Apply min / max --------
        ApplyCurve(dnc.sunDiscMultiplier, discMult, out dnc.sunDiscMultiplierMin, out dnc.sunDiscMultiplierMax);
        ApplyCurve(dnc.sunDiscExponent, discExp, out dnc.sunDiscExponentMin, out dnc.sunDiscExponentMax);
        ApplyCurve(dnc.sunHaloExponent, haloExp, out dnc.sunHaloExponentMin, out dnc.sunHaloExponentMax);
        ApplyCurve(dnc.sunHaloContribution, haloCon, out dnc.sunHaloContributionMin, out dnc.sunHaloContributionMax);
        ApplyCurve(dnc.horizonExponent, horExp, out dnc.horizonExponentMin, out dnc.horizonExponentMax);
        ApplyCurve(dnc.horizonContribution, horCon, out dnc.horizonContributionMin, out dnc.horizonContributionMax);
        ApplyCurve(dnc.skyGradientExponent, skyExp, out dnc.skyGradientExponentMin, out dnc.skyGradientExponentMax);

        // -------- Gradients --------
        BakeGradient(dnc.sunDiscColor, "_SunDiscColor");
        BakeGradient(dnc.sunHaloColor, "_SunHaloColor");
        BakeGradient(dnc.horizonColor, "_HorizonLineColor");
        BakeGradient(dnc.skyTopColor, "_SkyGradientTop");
        BakeGradient(dnc.skyBottomColor, "_SkyGradientBottom");

        EditorUtility.SetDirty(dnc);
        Debug.Log("SkyboxCycleHelper: Bake successful");
    }

    // ---------------- HELPERS ----------------

    private List<(float t, float v)> Collect(string prop)
    {
        var list = new List<(float, float)>();
        foreach (var e in skyboxes)
            list.Add((Mathf.Repeat(e.Time, 1f), e.Skybox.GetFloat(prop)));
        return list;
    }

    private void ApplyCurve(
        AnimationCurve curve,
        List<(float t, float v)> raw,
        out float min,
        out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;

        foreach (var r in raw)
        {
            min = Mathf.Min(min, r.v);
            max = Mathf.Max(max, r.v);
        }

        if (Mathf.Approximately(min, max))
            max = min + 0.0001f;

        curve.keys = Array.Empty<Keyframe>();

        foreach (var r in raw)
        {
            float normalized = Mathf.InverseLerp(min, max, r.v);
            curve.AddKey(new Keyframe(r.t, normalized));
        }

        for (int i = 0; i < curve.length; i++)
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
    }

    private void BakeGradient(Gradient gradient, string prop)
    {
        var colors = new List<GradientColorKey>();
        var alphas = new List<GradientAlphaKey>();

        foreach (var e in skyboxes)
        {
            float t = Mathf.Repeat(e.Time, 1f);
            Color c = e.Skybox.GetColor(prop);
            colors.Add(new GradientColorKey(c, t));
            alphas.Add(new GradientAlphaKey(1, t));
        }

        gradient.SetKeys(colors.ToArray(), alphas.ToArray());
    }
#endif
}