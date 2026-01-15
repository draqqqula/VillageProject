using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform _destination;

    [SerializeField]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var velocity = other.gameObject.GetComponent<CharacterVelocity>();
            var controller = other.gameObject.GetComponent<CharacterController>();
            var aggregator = other.gameObject.GetComponent<InputAggregator>();
            velocity.enabled = false;
            controller.enabled = false;
            aggregator.enabled = false;
            other.transform.position = _destination.position;
            other.transform.rotation = _destination.rotation;
            velocity.enabled = true;
            controller.enabled = true;
            aggregator.enabled = true;
        }
    }
}
