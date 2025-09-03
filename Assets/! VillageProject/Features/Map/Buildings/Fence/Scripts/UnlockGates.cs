using UnityEngine;
using Zenject;

public class UnlockGates : MonoBehaviour
{
    [Inject] private WaveController _waveController;
    [SerializeField] private GameObject _trigger;
    [SerializeField] private BoxCollider _collider;

    private void Awake()
    {
        _waveController.BreakStarted.AddListener(() => 
        { 
            _trigger.SetActive(true); 
            _collider.enabled = false; 
        });
        _waveController.BreakFinished.AddListener(() => 
        { 
            _trigger.SetActive(false); 
            _collider.enabled = true; 
        });
    }
}
