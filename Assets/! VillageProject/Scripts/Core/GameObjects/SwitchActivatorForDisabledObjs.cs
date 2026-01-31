using UnityEngine;

public class SwitchActivatorForDisabledObjs : MonoBehaviour
{
    [SerializeField] private GameObject _opposite;
    private bool _isDisabledOnStart;

    private void OnEnable()
    {
        _isDisabledOnStart = !_opposite.activeSelf;
        _opposite.SetActive(false);
    }

    private void OnDisable()
    {
        if (_isDisabledOnStart) return;
        _opposite.SetActive(true);
    }
}