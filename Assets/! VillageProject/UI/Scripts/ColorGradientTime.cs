using System.Collections;
using TMPro;
using UnityEngine;

public class ColorGradientTime : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _duration = 1.0f;
    [SerializeField] private Gradient gradient;

    private void OnEnable()
    {
        StartCoroutine(ChangeColorGradualy());
    }

    private IEnumerator ChangeColorGradualy()
    {
        float t = 0;
        while (t < _duration)
        {
            _text.color = gradient.Evaluate(t / _duration);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _text.color = gradient.Evaluate(1);
    }
}
