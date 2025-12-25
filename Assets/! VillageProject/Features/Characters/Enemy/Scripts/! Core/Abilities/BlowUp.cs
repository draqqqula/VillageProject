using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BlowUp : MonoBehaviour
{
    [Header("Ability Parameters")]
    [SerializeField] private float _chargingDuration = 3f;
    [SerializeField] private float _blowUpDuration = 2f;
    [SerializeField] private float _vfxDelay;
    
    [SerializeField] private GameObject _blowUpHitboxPrefab;
    private GameObject _blowUpHitbox;
    
    private Coroutine _coroutine;
    private IInstantiator _instantiator;
    
    public bool IsBlowing { get; private set; }

    public event Action OnStarted;
    public event Action OnFinished;
    public event Action OnFinishedCooldown;

    [Inject]
    private void Construct(IInstantiator instatiator)
    {
        _instantiator = instatiator;
        IsBlowing = false;
    }
    
    public void ActivateBlowUp()
    {
        if (_coroutine != null || IsBlowing) return;
        _coroutine = StartCoroutine(BattleCryRoutine());
    }

    private IEnumerator BattleCryRoutine()
    {
        OnStarted?.Invoke();
        IsBlowing = true;
        
        yield return new WaitForSeconds(_chargingDuration);
        yield return new WaitForSeconds(_vfxDelay);

        _blowUpHitbox = _instantiator.InstantiatePrefab(_blowUpHitboxPrefab, transform.position, Quaternion.identity, null);

        _blowUpHitbox.transform.SetParent(null);
        yield return new WaitForSeconds(_blowUpDuration);
        
        Destroy(_blowUpHitbox);
        _blowUpHitbox = null;
        _coroutine = null;
        OnFinished?.Invoke();
        
        Destroy(gameObject);
    }

    public void Interrupt()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
            if (_blowUpHitbox)
            {
                Destroy(_blowUpHitbox);
                _blowUpHitbox = null;
                OnFinished?.Invoke();
            }
        }
    }
}
