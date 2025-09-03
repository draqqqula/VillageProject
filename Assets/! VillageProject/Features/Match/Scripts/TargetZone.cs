using UnityEngine;
using Zenject;

public class TargetZone : MonoBehaviour
{
    private const string TargetTag = "damageable";

    [Inject] private MatchObjective _objective;
    [Inject] private WaveController _waveController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TargetTag) && other.TryGetComponent<TargetDamage>(out var damage))
        {
            _objective.Take(damage);
            _waveController.HandleUnitDied();
            Destroy(other.gameObject);
        }
    }
}
