using UnityEngine;
using Zenject;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform _destination;
    [SerializeField] private Anchor _anchor;
    
    [SerializeField] private GameObject _teleportAnchorsEnabler;
    [SerializeField] private GameObject _tip;
    [SerializeField] private AnchorMode _portalMode;
    
    [Inject] private FirstPersonController _player;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _portalMode.DefaultAnchor = _anchor;
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
        _player.UpdateRotation(_player.transform.rotation.eulerAngles, Vector3.zero);
        
        velocity.enabled = true;
        controller.enabled = true;
        aggregator.enabled = true;
    }
}