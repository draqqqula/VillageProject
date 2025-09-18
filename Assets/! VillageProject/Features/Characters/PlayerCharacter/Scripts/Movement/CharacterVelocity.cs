using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterVelocity : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    
    private Vector3 _velocity;
    private ReactiveProperty<Vector3> _velocityProperty = new ReactiveProperty<Vector3>();
    public ReadOnlyReactiveProperty<Vector3> Velocity => _velocityProperty;
    
    private List<Vector3> _sources = new List<Vector3>();
    
    private void Reset()
    {
        _characterController = GetComponent<CharacterController>();
    }
    
    private void FixedUpdate()
    {
        _velocityProperty.Value = _velocity;
        if (_velocity != Vector3.zero)
        {
            _characterController.Move(_velocity);
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
