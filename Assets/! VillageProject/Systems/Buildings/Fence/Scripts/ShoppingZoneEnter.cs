using R3;
using R3.Triggers;
using UnityEngine;
using Zenject;

public class ShoppingZoneEnter : MonoBehaviour
{
    [Inject] private ShoppingZone _shoppingZone;
    [SerializeField] private PlayerTrigger _gateTrigger;
    [SerializeField] private PlayerTrigger _zoneTrigger;

    private void Awake()
    {
        _gateTrigger.PlayerInside.Subscribe(HandlePlayerInside);
        _shoppingZone.PlayerInside.Value = true;
    }

    private void HandlePlayerInside(bool value)
    {
        if (!value)
        {
            if (_zoneTrigger.PlayerInside.CurrentValue)
            {
                _shoppingZone.PlayerInside.Value = true;
                return;
            }
            _shoppingZone.PlayerInside.Value = false;
        }
    }
}
