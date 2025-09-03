using UnityEngine;

public class GiveResourceOnDeath : MonoBehaviour
{
    [field: SerializeField] public ResourceVariable Resource { get; private set; }
    [SerializeField] private DeathEvent _deathEvent;
    [SerializeField] private uint _amount;
    private void Give()
    {
        Resource.Increment(_amount);
    }

    private void Reset()
    {
        _deathEvent = GetComponent<DeathEvent>();
    }

    private void OnEnable()
    {
        _deathEvent.FiredEvent += Give;
    }

    private void OnDisable()
    {
        _deathEvent.FiredEvent -= Give;
    }
}
