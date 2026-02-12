using R3;
using UnityEngine;
using Zenject;

public class UnlockGates : MonoBehaviour
{
    [Inject] private WaveController _waveController;
    [SerializeField] private GameObject _trigger;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private ShoppingZone _shoppingZone;

    private void Awake()
    {
        _waveController.BreakStarted.AddListener(OnBreakStarted);
        _waveController.BreakFinished.AddListener(OnBreakFinished);

        _shoppingZone.PlayerInside.Subscribe(OnPlayerMoveThroughGates).AddTo(this);
    }

    private void OnBreakStarted()
    {
        _trigger.SetActive(true); 
        _collider.enabled = false; 
    }

    private void OnBreakFinished()
    {
        if (!_shoppingZone.PlayerInside.CurrentValue)
        {
            _trigger.SetActive(false); 
            _collider.enabled = true; 
        }
    }

    private void OnPlayerMoveThroughGates(bool playerInside)
    {
        if (!playerInside && !_waveController.IsOnBreak.CurrentValue)
        {
            OnBreakFinished();
        }
    }

    private void OnDestroy()
    {
        _waveController.BreakStarted.RemoveListener(OnBreakStarted);
        _waveController.BreakFinished.RemoveListener(OnBreakFinished);
    }
}
