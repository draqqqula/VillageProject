using System.Collections;
using UnityEngine;
using Zenject;

public class ParryDisplay : MonoBehaviour
{
    [Inject] private ParryingUpdater _updater;
    [SerializeField] private GameObject _display;

    private void Update()
    {
        if (_updater.Parrying.CurrentValue && !_display.activeSelf)
        {
            _display.SetActive(true);
        }
        else if (!_updater.Parrying.CurrentValue && _display.activeSelf)
        {
            _display.SetActive(false);
        }
    }
}