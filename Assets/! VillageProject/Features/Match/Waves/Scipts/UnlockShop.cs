using R3;
using UnityEngine;
using Zenject;

public class UnlockShop : MonoBehaviour
{
    [Inject] private ShoppingZone _shoppingZone;
    [SerializeField] private EnterOnlyInputActiob _inputListener;

    private void Awake()
    {
        _shoppingZone.PlayerInside.Subscribe(HandlePlayerInside).AddTo(this);
    }
    
    private void HandlePlayerInside(bool value)
    {
        //_inputListener.enabled = value;
    }
}
