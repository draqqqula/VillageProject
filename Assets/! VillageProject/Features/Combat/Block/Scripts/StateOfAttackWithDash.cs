using Zenject;

public sealed class StateOfAttackWithDash : StateOfAttack
{
    [Inject(Id = "Dash")] private IAnimationWindowListener _dashAttackWindow;

    protected override void Start()
    {
        base.Start();
        _dashAttackWindow.OnEnter += OnAttack;
        _dashAttackWindow.OnExit += OnAttack;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _dashAttackWindow.OnEnter -= OnAttack;
        _dashAttackWindow.OnExit -= OnAttack;
    }
}