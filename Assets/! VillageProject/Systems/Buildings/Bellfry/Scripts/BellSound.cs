using System.Collections;
using UnityEngine;

public class BellSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioWeak;
    [SerializeField] private AudioSource _audioStrong;

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
    }
}