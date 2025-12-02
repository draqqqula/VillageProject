using R3;
using System.Collections;
using UnityEngine;
using Zenject;

public class ParryDisplay : MonoBehaviour
{
    private const string AnimationParameter = "t";

    [Inject] private ParryingUpdater _updater;
    [SerializeField] private Animator _display;

    private void Awake()
    {
        _updater.Parrying.Subscribe(HandleParrying).AddTo(this);
    }

    private void HandleParrying(bool value)
    {
        if (value)
        {
            _display.gameObject.SetActive(true);
            StartCoroutine(UpdateDisplay());
        }
        else
        {
            StopAllCoroutines();
            _display.gameObject.SetActive(false);
        }
    }

    private IEnumerator UpdateDisplay()
    {
        while (true)
        {
            _display.SetFloat(AnimationParameter, _updater.GetCurrentParryingTime());
            yield return new WaitForEndOfFrame();
        }
    }
}