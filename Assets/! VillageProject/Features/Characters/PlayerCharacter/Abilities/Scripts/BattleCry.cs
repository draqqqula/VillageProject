using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BattleCry : InputListener
{
    [Header("Ability Parameters")]
    [SerializeField] private float _abilityDuration = 5f;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _vfxDelay;

    [Header("Adrenaline Parameters")]
    [SerializeField] private float _adrenalineValue;
    [SerializeField] private float _adrenalineCooldown;
    [SerializeField] private Adrenaline _adrenaline;
    
    [SerializeField, FromInputActionAsset("BattleCry")] private InputActionReference _battleCry;
    
    [SerializeField] private TriggerTargetDetection _battleCryHitboxPrefab;
    private Target _playerTarget;
    private Camera _camera;
    private Coroutine _coroutine;
    private IInstantiator _instantiator;
    
    private bool _isOnCooldown = false;

    public event Action OnStarted;
    public event Action OnFinished;
    public event Action OnFinishedCooldown;

    [Inject]
    private void Construct(IInstantiator instatiator)
    {
        _instantiator = instatiator;
        _playerTarget = GetComponent<Target>();
        _camera = Camera.main;
    }
    
    private void OnEnable()
    {
        _battleCry.action.performed += ActivateBattleCry;
    }

    private void OnDisable()
    {
        _battleCry.action.performed -= ActivateBattleCry;
    }

    private void ActivateBattleCry(InputAction.CallbackContext context)
    {
        if (_coroutine != null || _isOnCooldown) return;
        _coroutine = StartCoroutine(BattleCryRoutine());
    }

    private IEnumerator BattleCryRoutine()
    {
        OnStarted?.Invoke();

        yield return new WaitForSeconds(_vfxDelay);

        var battleCryHitbox = _instantiator.InstantiatePrefabForComponent<TriggerTargetDetection>(_battleCryHitboxPrefab,
            transform.position, Quaternion.identity, null);
        
        battleCryHitbox.transform.SetParent(null);
        battleCryHitbox.SetTarget(_playerTarget);
        battleCryHitbox.transform.rotation = Quaternion.Euler(new Vector3(0, _camera.transform.rotation.eulerAngles.y, 
            _camera.transform.rotation.eulerAngles.z));
        
        _adrenaline.Gain(_adrenalineValue, _adrenalineCooldown);
        
        yield return new WaitForSeconds(_abilityDuration);
        
        Destroy(battleCryHitbox.gameObject);
        _coroutine = null;
        OnFinished?.Invoke();
        
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(_cooldown);
        _isOnCooldown = false;
        OnFinishedCooldown?.Invoke();
    }
}