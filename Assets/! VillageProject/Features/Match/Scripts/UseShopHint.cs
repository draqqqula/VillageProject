using R3.Triggers;
using R3;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class UseShopHint : MonoBehaviour
{
    [Inject] private ShoppingZone _shoppingZone;

    public UnityEvent Show;
    public UnityEvent Hide;

    private void Awake()
    {
        _shoppingZone.PlayerInside
            .Subscribe(InvokeEvent)
            .AddTo(this);
    }

    private void InvokeEvent(bool value)
    {
        if (value)
        {
            Show?.Invoke();
        }
        else
        {
            Hide?.Invoke();
        }
    }
}
