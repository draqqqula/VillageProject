using UnityEngine;
using UnityEngine.Events;

public class TravellingProjectile : MonoBehaviour
{
    public UnityEvent DestinationReached;
    [SerializeField] private float _speed = 1f;
    private float _traveled = 0;

    [field: SerializeField] public Transform Source { get; private set; }
    [field: SerializeField] public Transform Destination { get; private set; }

    public void SetPath(Transform source, Transform destination)
    {
        Source = source;
        Destination = destination;
    }

    void FixedUpdate()
    {
        if (Source == null || Destination == null)
        {
            return;
        }

        var distance = Destination.position - Source.position;
        _traveled = Mathf.Clamp(_traveled + _speed * Time.fixedDeltaTime, 0, distance.magnitude);

        transform.position = Source.position + (Destination.position - Source.position) * (_traveled / distance.magnitude);
        transform.LookAt(Destination.position);

        if (_traveled == distance.magnitude)
        {
            DestinationReached?.Invoke();
            enabled = false;
        }
    }
}
