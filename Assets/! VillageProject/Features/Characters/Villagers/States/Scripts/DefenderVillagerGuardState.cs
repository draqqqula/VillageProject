using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

public sealed class DefenderVillagerGuardState : GuardVillagerState, IUpdatableState
{
    private const float AttackDistance = 2;
    
    private SearchForTarget _searchForTarget;
    private NavmeshMovementAgent _navmeshAgent;
    private Profession _profession;
    private AttackBonus _attackBonus;
    
    private VillagerMovementHandler _movementHandler;
    private Vector3 _prevPos;
    
    private Animator _animator;
    private Coroutine _coroutine;
    
    private RaiseExperienceHandler _experienceHandler;
    
    public DefenderVillagerGuardState(NavmeshMovementAgent navmeshAgent, SearchForTarget searchForTarget, Animator animator, Profession profession,
        GameTimer gameTimer, AttackBonus attackBonus)
    {
        _searchForTarget = searchForTarget;
        _animator = animator;
        _profession = profession;
        _attackBonus = attackBonus;
        
        _navmeshAgent = navmeshAgent;
        _movementHandler = new VillagerMovementHandler(navmeshAgent);

        _experienceHandler = new RaiseExperienceHandler(profession, gameTimer);
        
        _profession.Experience.Subscribe(TryRiseAttack).AddTo(_navmeshAgent.gameObject);
    }
    
    public override void EnterState()
    { 
        _animator.SetBool("Agressed", true);
        _experienceHandler.StartRaisingExperience();
    }

    private void ActivateMovement()
    {
        _prevPos = _searchForTarget.MainTarget.transform.position;
        _movementHandler.ActivateMovement(_prevPos);
    }
    
    public void Update()
    {
        if (_searchForTarget.MainTarget == null) return;
        
        if (Vector3.Distance(_navmeshAgent.transform.position, _searchForTarget.MainTarget.transform.position) <= AttackDistance)
        {
            _animator.SetTrigger("Attack");
        }
        else
        {
            ActivateMovement();
        }
    }
    
    private void TryRiseAttack(float experience)
    {
        var multiplier = (_profession.ProfessionData as DefenderProfessionData).DamageMultiplierCurve.Evaluate(experience);
        _attackBonus.DamageMultiplier = _attackBonus.DefaultDamageMultiplier * multiplier;
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _movementHandler.DeactivateMovement();
        
        _experienceHandler.StopRaisingExperience();
        _animator.SetBool("Agressed", false);
        _animator.ResetTrigger("Attack");

        if (_coroutine != null)
        {
            _navmeshAgent.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}