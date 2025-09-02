using R3.Triggers;
using R3;
using UnityEngine;
using Zenject;
using UnityEngine.Events;

public class NavigateToGatesHint : MonoBehaviour
{
    [Inject] private AnchorMode _anchorMode;
    [Inject] private ShoppingZone _shoppingZone;
    private ReactiveProperty<bool> _shopCheckedFlag = new ReactiveProperty<bool>(false);

    public UnityEvent Show;
    public UnityEvent Hide;

    private void Awake()
    {
        _anchorMode.OnEnableAsObservable()
            .Subscribe(_ => _shopCheckedFlag.Value = true)
            .AddTo(this);
        _shoppingZone.PlayerInside
            .Subscribe(it => _shopCheckedFlag.Value = false)
            .AddTo(this);
        _shopCheckedFlag
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
