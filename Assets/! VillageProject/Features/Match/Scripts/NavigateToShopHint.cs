using UnityEngine;
using UnityEngine.Events;
using Zenject;
using R3;
using R3.Triggers;

public class NavigateToShopHint : MonoBehaviour
{
    [Inject] private WaveController _waveController;
    [Inject] private ShoppingZone _shoppingZone;

    public UnityEvent Show;
    public UnityEvent Hide;

    private void Awake()
    {
        _waveController.IsOnBreak
            .CombineLatest(_shoppingZone.PlayerInside, GetResult)
            .Subscribe(HandleStateChanged)
            .AddTo(this);
    }

    private void HandleStateChanged(bool value)
    {
        if (value)
        {
            Show.Invoke();
        }
        else
        {
            Hide.Invoke();
        }
    }

    private bool GetResult(bool value, bool b)
    {
        return _waveController.IsOnBreak.CurrentValue && !_shoppingZone.PlayerInside.Value;
    }
}
