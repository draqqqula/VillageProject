using Unity.Behavior;
using UnityEngine;
using Zenject;

public class Dash : MonoBehaviour
{
    [Inject(Id = "Dash")] IAnimationWindowListener _dashWindow;
    [Inject(Id = "Stop")] IAnimationWindowListener _stopWindow;
    [SerializeField] private NavmeshMovementAgent _agent;
    
    [SerializeField] private GameObject _player;
    [SerializeField] private Speed _speed;
    [SerializeField] private GameObject _hitbox;
    
    private IWorkEventSource<WorkResult> _source;
    private bool _isFinished = true;
    
    public bool IsDashing {get; private set;}
    public bool IsCharging {get; private set;}
    
    private void Start()
    {
        _dashWindow.OnEnter += UseDash;
        _stopWindow.OnEnter += Stop;
        _stopWindow.OnExit += Move;
    }

    private void Stop()
    {
        IsCharging = true;
        _agent.StopAgent();
        _agent.CanStop = false;
        _agent.CanChangeDestination = false;
    }

    private void Move()
    {
        IsCharging = false;
        _agent.CanStop = true;
        _agent.CanChangeDestination = true;
    }

    private void UseDash()
    {
        if (IsDashing) return;
        IsDashing = true;
        Debug.Log("Dash");

        _agent.TrySetInstructions(_player.transform.position, out var source);
        BindWork(source);
    }

    private void HandleFinished(WorkResult result)
    {
        UnbindWork();
    }
    
    private void BindWork(IWorkEventSource<WorkResult> source)
    {
        _isFinished = false;
        _agent.CanStop = false;
        _agent.CanChangeDestination = false;
        
        _source = source;
        _source.OnFinished += HandleFinished;
        
        _speed.Value.Value *= 6f;
        _hitbox.gameObject.SetActive(true);
    }
    
    private void UnbindWork()
    {
        if (_isFinished) return;
        Debug.Log("Unbind");
        
        _agent.CanStop = true;
        _agent.CanChangeDestination = true;
        
        _hitbox.gameObject.SetActive(false);
        _speed.Value.Value /= 6f;
        
        _source.OnFinished -= HandleFinished;
        
        _source = null;
        _isFinished = true;
        IsDashing = false;
    }

    private void OnDestroy()
    {
        _dashWindow.OnEnter -= UseDash;
        _stopWindow.OnEnter -= Stop;
        _stopWindow.OnExit -= Move;
        
        UnbindWork();
    }
}
