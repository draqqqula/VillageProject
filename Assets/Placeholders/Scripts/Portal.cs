using UnityEngine;
using Zenject;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform _destination;
    [SerializeField] private GameObject _teleportAnchorsEnabler;
    [SerializeField] private GameObject _tip;
    [Inject] private FirstPersonController _player;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _teleportAnchorsEnabler.SetActive(true);
            _tip.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _teleportAnchorsEnabler.SetActive(false);
            _tip.SetActive(false);
        }
    }

    public void Teleport()
    {
        var velocity = _player.GetComponent<CharacterVelocity>();
        var controller = _player.GetComponent<CharacterController>();
        var aggregator = _player.GetComponent<InputAggregator>();
        velocity.enabled = false;
        controller.enabled = false;
        aggregator.enabled = false;
        _player.transform.position = _destination.position;
        _player.transform.rotation = _destination.rotation;
        velocity.enabled = true;
        controller.enabled = true;
        aggregator.enabled = true;
    }
}