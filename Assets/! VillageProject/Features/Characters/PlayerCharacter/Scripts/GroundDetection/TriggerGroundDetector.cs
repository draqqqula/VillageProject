using System.Collections;
using UnityEngine;

public class TriggerGroundDetector : GroundDetectorBase
{
    [SerializeField] private CharacterController _characterController;
    public override bool IsGrounded => _characterController.isGrounded;
}