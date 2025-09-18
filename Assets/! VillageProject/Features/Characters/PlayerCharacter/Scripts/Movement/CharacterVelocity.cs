using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterVelocity : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    private Vector3 _velocity;
    private List<Vector3> _sources = new List<Vector3>();
    [SerializeField] private CameraShaking _cameraShaking;
    
    private void Reset()
    {
        _characterController = GetComponent<CharacterController>();
    }
    
    private void FixedUpdate()
    {
        if (_velocity != Vector3.zero)
        {
            _characterController.Move(_velocity);
            var velocityCopy = new Vector3(_velocity.x, 0, _velocity.z);
            _cameraShaking.Shake(velocityCopy.magnitude);
        }

        _velocity = Vector3.zero;
        _sources.Clear();
    }

    public void Add(Vector3 velocity)
    {
        _velocity += velocity;
        _sources.Add(velocity);
    }
}
