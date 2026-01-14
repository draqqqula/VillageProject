using System;
using System.Collections;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using Zenject;

public class Dash : MonoBehaviour
{
    [Inject(Id = "Dash")] IAnimationWindowListener _dashWindow;
    [Inject(Id = "Stop")] IAnimationWindowListener _stopWindow;
    
    [SerializeField] private MovementSwitcher _switcher;
    private MovementWorkerBase _prevMovementAgent;
    private ManualVelocityMovementAgent _agent;
    
    [SerializeField] private Speed _speed;
    [SerializeField] private float _speedMultiplier = 2;
    
    [SerializeField] private float _dashTime;
    [SerializeField] private AnimationCurve _velocityByTimeCurve;
    
    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _knockbackSpeed;
    [SerializeField] private float _knockbackTime;
    
    [SerializeField] private GameObject _hitbox;
    private CollideHitboxEvent _hitboxEvent;
    private GameObject _player;
    
    private const float HitboxDelay = 0.2f;
    private Coroutine _delayCoroutine;
    private Coroutine _cooldownCoroutine;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Collider _collider;
    
    private bool _isKnockedBack;
    public bool IsDashing {get; private set;}
    public bool IsCharging {get; private set;}
    public bool IsCooldown {get; private set;}

    [Inject]
    private void Construct(FirstPersonController player)
    {
        _player = player.gameObject;
        _dashWindow.OnEnter += UseDash;
        _stopWindow.OnEnter += Stop;
        _stopWindow.OnExit += Move;
        
        _hitboxEvent = _hitbox.GetComponent<CollideHitboxEvent>();
        _agent = new ManualVelocityMovementAgent(_velocityByTimeCurve, _characterController);
    }
    
    private void Stop()
    {
        IsCharging = true;
        transform.LookAt(_player.transform);
    }

    private void Move()
    {
        IsCharging = false;
    }

    private void Update()
    {
        _agent.Update();
    }

    private void UseDash()
    {
        if (IsDashing) return;
        
        transform.LookAt(_player.transform);
        var agent = _switcher.GetWorkers().FirstOrDefault(ag => ag.enabled);
        _prevMovementAgent = agent;
        agent.enabled = false;
        
        _speed.Value.Value *= _speedMultiplier;
        _agent.StartMovement(_player.transform.position, _speed.Value.Value, _dashTime);
        OnDashStarted();
    }
    
    private void OnDashStarted()
    {
        IsDashing = true;
        
        _agent.OnFinished += OnDashFinished;
        _collider.enabled = false;
        _hitbox.gameObject.SetActive(true);
        _hitboxEvent.OnHit += TryInterruptDash;
    }

    private void TryInterruptDash(GameObject hittedObj)
    {
        int[] layers = new int[4]
        {
            LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("TargetDetector"), LayerMask.NameToLayer("Enemy"),
            LayerMask.NameToLayer("Demon")
        }; 
        
        if (layers.All(l => l != hittedObj.layer))
        {
            _agent.OnFinished -= OnDashFinished;
            var knockbackDirection = _agent.CurrentVelocity.normalized * -1;
            _agent.StopMovement();
            Knockback(knockbackDirection);
        }
    }

    private void Knockback(Vector3 knockbackDirection)
    {
        _isKnockedBack = true;
        _agent.OnFinished += OnKnockbackFinished;
        _agent.StartMovement(transform.position + knockbackDirection * _knockbackForce, _knockbackSpeed, _knockbackTime, false);
        _animator.SetTrigger("Knocked");
    }

    private void OnKnockbackFinished()
    {
        _isKnockedBack = false;
        _agent.OnFinished -= OnKnockbackFinished;
        _animator.ResetTrigger("Knocked");
        OnDashFinished();
    }
    
    private void OnDashFinished()
    {
        if (!IsDashing) return;
        
        _animator.SetBool("Dash", false);
        
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        _delayCoroutine = StartCoroutine(DelayHitboxCoroutine());
        
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        
        _speed.ReturnToDefault();
        _agent.OnFinished -= OnDashFinished;
        
        _prevMovementAgent.enabled = true;
        _prevMovementAgent = null;
        
        IsDashing = false;
    }
    
    private IEnumerator DelayHitboxCoroutine()
    {
        yield return new WaitForSeconds(HitboxDelay);
        _hitbox.gameObject.SetActive(false);
        _collider.enabled = true;
        _hitboxEvent.OnHit -= TryInterruptDash;
        _delayCoroutine = null;
    }

    private IEnumerator CooldownCoroutine()
    {
        IsCooldown = true;
        yield return null;
        IsCooldown = false;
    }

    private void OnDestroy()
    {
        if (_hitbox.activeInHierarchy) _hitboxEvent.OnHit -= TryInterruptDash;
        _dashWindow.OnEnter -= UseDash;
        _stopWindow.OnEnter -= Stop;
        _stopWindow.OnExit -= Move;
        
        if (_isKnockedBack) OnKnockbackFinished();
        else OnDashFinished();
    }
}