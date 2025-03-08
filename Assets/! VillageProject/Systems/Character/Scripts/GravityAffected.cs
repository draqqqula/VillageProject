using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GravityAffected : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _gravity;

    private void Reset()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        var velocity = Vector3.down * _gravity;
        _characterController.Move(velocity);
    }
}
