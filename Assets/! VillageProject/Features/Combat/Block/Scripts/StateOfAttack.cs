using UnityEngine;
using Zenject;

public class StateOfAttack : MonoBehaviour
{
    [field: SerializeField] public bool IsBlocking { get; set; }
    [Inject(Id = "SlashAttack")]  private IAnimationWindowListener _slashAttackWindow;

    private void Start()
    {
        _slashAttackWindow.OnEnter += OnSlashAttack;
        _slashAttackWindow.OnExit += OnSlashAttack;
    }
    
    private void OnSlashAttack()
    {
        IsBlocking = false;
    }

    private void OnDestroy()
    {
        _slashAttackWindow.OnEnter -= OnSlashAttack;
        _slashAttackWindow.OnExit -= OnSlashAttack;
    }
}