using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class WalkingSounds : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private AudioSource _walkSoundSource;
    [SerializeField] private List<AudioClip> _walkingSounds;
    [SerializeField] private float _interval;
    private Coroutine _walkingSoundCoroutine;

    [SerializeField, Range(0, 1)] private float _soundsStartTiming;
    [SerializeField, Range(0, 1)] private float _soundsDuration;
    
    [Inject] private MoveParamUpdater _moveParamUpdater;

    void Update()
    {
        if (!_moveParamUpdater.IsParamOnRange(_soundsStartTiming, _soundsStartTiming + _soundsDuration, false) 
            || !_characterController.isGrounded)
        {
            StopWalkingSounds();
        }
        else
        {
            if (_walkingSoundCoroutine == null)
            {
                _walkingSoundCoroutine = StartCoroutine(Walk());
            }
        }
    }

    private IEnumerator Walk()
    {
        var currentSound = 0;
        while (true)
        {
            yield return new WaitForSeconds(_interval);
            _walkSoundSource.clip = _walkingSounds[currentSound];
            _walkSoundSource.Play();
            currentSound++;
            if (currentSound == _walkingSounds.Count)
            {
                currentSound = 0;
            }
        }
    }

    private void StopWalkingSounds()
    {
        if (_walkingSoundCoroutine != null)
        {
            StopCoroutine(_walkingSoundCoroutine);
            _walkingSoundCoroutine = null;
        }
    }
}