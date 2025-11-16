using UnityEngine;

[System.Serializable]
public class NormalizedAnimationCurve
{
    [SerializeField]
    private AnimationCurve curve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(1f, 1f)
    );

    public AnimationCurve Curve
    {
        get => curve;
        set
        {
            curve = value;
            ValidateEndpoints();
        }
    }

    public void ValidateEndpoints()
    {
        if (curve.length == 0)
        {
            curve.AddKey(new Keyframe(0f, 0f));
            curve.AddKey(new Keyframe(1f, 1f));
            return;
        }

        var first = curve.keys[0];
        if (!Mathf.Approximately(first.time, 0f))
        {
            curve.MoveKey(0, new Keyframe(0f, first.value, first.inTangent, first.outTangent));
        }

        int lastIdx = curve.length - 1;
        var last = curve.keys[lastIdx];
        if (!Mathf.Approximately(last.time, 1f))
        {
            curve.MoveKey(lastIdx, new Keyframe(1f, last.value, last.inTangent, last.outTangent));
        }
    }

    public void AddKey(Keyframe key)
    {
        curve.AddKey(key);
        ValidateEndpoints();
    }

    public void RemoveKey(int index)
    {
        if (index <= 0 || index >= curve.length - 1)
            return;

        curve.RemoveKey(index);
        ValidateEndpoints();
    }

    public float Evaluate(float time)
    {
        return curve.Evaluate(time);
    }
}