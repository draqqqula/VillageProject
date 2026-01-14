using UnityEngine;
using Zenject;

public class StateOfAttack : MonoBehaviour
{
    [field: SerializeField] public bool IsBlocking { get; set; }
    [Inject(Id = "SlashAttack")]  private IAnimationWindowListener _slashAttackWindow;

    protected virtual void Start()
    {
        _slashAttackWindow.OnEnter += OnAttack;
        _slashAttackWindow.OnExit += OnAttack;
    }
    
    protected void OnAttack()
    {
        IsBlocking = false;
    }

    protected virtual void OnDestroy()
    {
        _slashAttackWindow.OnEnter -= OnAttack;
        _slashAttackWindow.OnExit -= OnAttack;
    }
}