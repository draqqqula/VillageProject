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
    
    [SerializeField] private GameObject _hitbox;
    private CollideHitboxEvent _hitboxEvent;
    private GameObject _player;
    
    private const float HitboxDelay = 0.2f;
    private Coroutine _delayCoroutine;
    private Coroutine _cooldownCoroutine;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    
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
        _agent = new ManualVelocityMovementAgent(_velocityByTimeCurve, _dashTime, _characterController);
    }
    
    private void Stop()
    {
        IsCharging = true;
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
        
        var agent = _switcher.GetWorkers().FirstOrDefault(ag => ag.enabled);
        _prevMovementAgent = agent;
        agent.enabled = false;
        
        _speed.Value.Value *= _speedMultiplier;
        _agent.StartMovement(_player.transform.position, _speed.Value.Value);
        BindWork();
    }
    
    private void BindWork()
    {
        IsDashing = true;
        
        _agent.OnFinished += UnbindWork;
        _hitbox.gameObject.SetActive(true);
        _hitboxEvent.OnHit += TryInterruptDash;
    }

    private void TryInterruptDash(GameObject hittedObj)
    {
        Debug.Log(hittedObj.name);
        if (hittedObj.layer != LayerMask.NameToLayer("Player"))
        {
            _agent.StopMovement();
            UnbindWork();
        }
    }
    
    private void UnbindWork()
    {
        if (!IsDashing) return;
        
        _animator.SetBool("Dash", false);
        
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        _delayCoroutine = StartCoroutine(DelayHitboxCoroutine());
        
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        
        _speed.ReturnToDefault();
        _agent.OnFinished -= UnbindWork;
        
        _prevMovementAgent.enabled = true;
        _prevMovementAgent = null;
        
        IsDashing = false;
    }
    
    private IEnumerator DelayHitboxCoroutine()
    {
        yield return new WaitForSeconds(HitboxDelay);
        _hitbox.gameObject.SetActive(false);
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
        
        UnbindWork();
    }
}