using System.Collections;
using UnityEngine;

public abstract class GroundDetectorBase : MonoBehaviour
{
    public virtual bool IsGrounded { get; }
}