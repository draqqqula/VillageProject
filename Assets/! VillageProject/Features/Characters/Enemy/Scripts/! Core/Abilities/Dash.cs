using System.Collections;
using Unity.Behavior;
using UnityEngine;
using Zenject;

public class Dash : MonoBehaviour
{
    [Inject(Id = "Dash")] IAnimationWindowListener _dashWindow;
    [Inject(Id = "Stop")] IAnimationWindowListener _stopWindow;
    [SerializeField] private NavmeshMovementAgent _agent;
    
    [SerializeField] private Speed _speed;
    private const float SpeedMultiplier = 6;
    
    [SerializeField] private GameObject _hitbox;
    private GameObject _player;
    
    private IWorkEventSource<WorkResult> _source;
    private const float HitboxDelay = 0.2f;
    private Coroutine _delayCoroutine;
    private Coroutine _cooldownCoroutine;
    
    [SerializeField] private Animator _animator;
    
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
    }
    
    private void Stop()
    {
        IsCharging = true;
    }

    private void Move()
    {
        IsCharging = false;
    }

    private void UseDash()
    {
        if (IsDashing) return;
        
        _agent.TrySetInstructions(_player.transform.position, out var source);
        BindWork(source);
    }

    private void HandleFinished(WorkResult result)
    {
        UnbindWork();
    }
    
    private void BindWork(IWorkEventSource<WorkResult> source)
    {
        IsDashing = true;
        _agent.CanStop = false;
        _agent.CanChangeDestination = false;
        
        _source = source;
        _source.OnFinished += HandleFinished;
        
        _speed.Value.Value *= SpeedMultiplier;
        _hitbox.gameObject.SetActive(true);
    }
    
    private void UnbindWork()
    {
        if (!IsDashing) return;
        
        _agent.CanStop = true;
        _agent.CanChangeDestination = true;
        _animator.SetBool("Dash", false);
        
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        _delayCoroutine = StartCoroutine(DelayHitboxCoroutine());
        
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        
        _speed.ReturnToDefault();
        _source.OnFinished -= HandleFinished;
        
        _source = null;
        IsDashing = false;
    }
    
    private IEnumerator DelayHitboxCoroutine()
    {
        yield return new WaitForSeconds(HitboxDelay);
        _hitbox.gameObject.SetActive(false);
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
        _dashWindow.OnEnter -= UseDash;
        _stopWindow.OnEnter -= Stop;
        _stopWindow.OnExit -= Move;
        
        UnbindWork();
    }
}
