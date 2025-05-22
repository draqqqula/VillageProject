using System.Collections;
using UnityEngine;

public class Bell : MonoBehaviour
{
    [SerializeField] private AudioSource _audioWeak;
    [SerializeField] private AudioSource _audioStrong;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Vector3 _force;

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Ring(int force)
    {
        if (force <= 0)
        {
            _audioWeak.Play();
        }
        else
        {
            _audioStrong.Play();
        }
        _rb.AddForce(_force * (force + 1));

    }

    [ContextMenu("Ring 1")]
    private void Ring1()
    {
        Ring(1);
    }

    [ContextMenu("Ring 2")]
    private void Ring2()
    {
        Ring(2);
    }
}