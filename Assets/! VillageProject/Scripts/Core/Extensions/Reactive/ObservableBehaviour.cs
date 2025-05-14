using R3;
using UnityEngine;

public class ObservableBehaviour : MonoBehaviour
{
    private ReactiveProperty<bool> _enabled = new ReactiveProperty<bool>(false);
    public ReadOnlyReactiveProperty<bool> Enabled => _enabled;

    protected void OnEnable()
    {
        _enabled.Value = true;
    }

    protected void OnDisable()
    {
        _enabled.Value = false;
    }
}