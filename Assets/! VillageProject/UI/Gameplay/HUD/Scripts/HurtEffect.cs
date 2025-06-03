using System.Collections;
using UnityEngine;

public class HurtEffect : MonoBehaviour
{
    [SerializeField] private CanvasGroup _effect;
    [SerializeField] private AnimationCurve _curve;

    public void Show()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeOpacityOverTime());
    }

    private IEnumerator ChangeOpacityOverTime()
    {
        float t = 0;
        while (t < 1)
        {
            _effect.alpha = _curve.Evaluate(t);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _effect.alpha = 0;
    }
}
