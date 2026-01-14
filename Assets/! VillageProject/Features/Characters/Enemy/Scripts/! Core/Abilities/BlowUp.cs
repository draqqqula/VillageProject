using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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
    
    public bool IsCharging { get; private set; }
    public bool IsBlowing { get; private set; }

    public event Action OnStarted;
    public event Action OnBlowedUp;
    public event Action OnHitboxDeleted;
    public event Action OnInterrupted;

    [Inject]
    private void Construct(IInstantiator instatiator)
    {
        _instantiator = instatiator;
    }
    
    public void ActivateBlowUpWithCharging()
    {
        if (IsBlowing || IsCharging) return;
        _coroutine = StartCoroutine(ChargingRoutine());
    }

    public void ActivateBlowUpImmediately()
    {
        if (IsBlowing) return;

        if (_coroutine != null && IsCharging)
        {
            IsCharging = false;
            StopCoroutine(_coroutine);
        }
        
        _coroutine = StartCoroutine(BlowUpRoutine());
    }
    
    private IEnumerator ChargingRoutine()
    {
        IsCharging = true;
        OnStarted?.Invoke();
        
        yield return new WaitForSeconds(_chargingDuration);
        yield return new WaitForSeconds(_vfxDelay);
        
        _coroutine = null;
        IsCharging = false;
        
        ActivateBlowUpImmediately();
    }

    private IEnumerator BlowUpRoutine()
    {
        IsBlowing = true;
        _blowUpHitbox = _instantiator.InstantiatePrefab(_blowUpHitboxPrefab, transform.position, Quaternion.identity, null);
        _blowUpHitbox.transform.SetParent(null);
        
        OnBlowedUp?.Invoke();
        yield return new WaitForSeconds(_blowUpDuration);
        
        Destroy(_blowUpHitbox);
        _blowUpHitbox = null;
        _coroutine = null;
        
        OnHitboxDeleted?.Invoke();
        IsBlowing = false;
        
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
                OnInterrupted?.Invoke();
            }
        }
    }
}
