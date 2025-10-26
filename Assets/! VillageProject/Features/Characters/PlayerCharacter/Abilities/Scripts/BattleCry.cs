using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BattleCry : InputListener
{
    private const float ABILITY_DURATION = 5f;
    [SerializeField] private float _cooldown;
    
    [SerializeField] private float _adrenalineValue;
    [SerializeField] private float _adrenalineCooldown;
    [SerializeField] private Adrenaline _adrenaline;
    
    [SerializeField, FromInputActionAsset("BattleCry")] private InputActionReference _battleCry;
    
    [SerializeField] private TriggerTargetDetection _battleCryHitboxPrefab;
    private Target _playerTarget;
    
    private Camera _camera;
    
    private Coroutine _coroutine;
    
    private bool _isOnCooldown = false;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public class BattleCryPerformedSignal
    {
        
    }

    public class BattleCryFinishedSignal
    {
        
    }

    public class BattleCryCooldownFinishedSignal
    {
        
    }
    
    private void Awake()
    {
        _playerTarget = GetComponent<Target>();
        _camera = Camera.main;
    }
    
    private void OnEnable()
    {
        _battleCry.action.performed += PerformBattleCry;
    }

    private void OnDisable()
    {
        _battleCry.action.performed -= PerformBattleCry;
    }

    private void PerformBattleCry(InputAction.CallbackContext context)
    {
        if (_coroutine != null || _isOnCooldown) return;
        _signalBus.Fire(new BattleCryPerformedSignal());
        _coroutine = StartCoroutine(BattleCryRoutine());
    }

    private IEnumerator BattleCryRoutine()
    {
        var battleCryHitbox = Instantiate(_battleCryHitboxPrefab, transform.position, Quaternion.identity, null);
        battleCryHitbox.SetTarget(_playerTarget);
        battleCryHitbox.transform.rotation = Quaternion.Euler(new Vector3(0, _camera.transform.rotation.eulerAngles.y, _camera.transform.rotation.eulerAngles.z));
        
        var rangeView = battleCryHitbox.GetComponentInChildren<BattleCryRangeView>(includeInactive: true);
        rangeView.ActivateView();
        
        _adrenaline.Gain(_adrenalineValue, _adrenalineCooldown);
        
        yield return new WaitForSeconds(ABILITY_DURATION);
        
        Destroy(battleCryHitbox.gameObject);
        _coroutine = null;
        _signalBus.Fire(new BattleCryFinishedSignal());
        
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(_cooldown);
        _isOnCooldown = false;
        _signalBus.Fire(new BattleCryCooldownFinishedSignal());
    }
}
