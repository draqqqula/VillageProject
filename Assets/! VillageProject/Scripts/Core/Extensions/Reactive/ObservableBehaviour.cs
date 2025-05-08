using R3;
using UnityEngine;

public class ObservableBehaviour : MonoBehaviour
{
    private ReactiveProperty<bool> _enabled = new ReactiveProperty<bool>(false);
    public ReadOnlyReactiveProperty<bool> Enabled => _enabled;

    private void OnEnable()
    {
        _enabled.Value = true;
    }

    private void OnDisable()
    {
        _enabled.Value = false;
    }
}