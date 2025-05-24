using System.Collections;
using UnityEngine;

public class Fade : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private CanvasGroup _group;

    private void OnEnable()
    {
        StartCoroutine(FadeInTime());
    }

    private IEnumerator FadeInTime()
    {
        var t = _duration;
        while (t > 0)
        {
            _group.alpha = t / _duration;
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _group.alpha = 0;
        _group.gameObject.SetActive(false);
    }
}
