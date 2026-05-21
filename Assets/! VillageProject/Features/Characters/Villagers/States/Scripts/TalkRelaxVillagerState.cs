using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TalkRelaxVillagerState : RelaxVillagerState, IUpdatableState
{
    private const float MinDistance = 20f;
    private const int MinVillagerTries = 7;
    private const float TalkDistance = 2f;
    private const float MinTryTiming = 5;
    private const float MaxRadius = 50; 
    
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerTransformHandler _transformHandler;
    
    private VillagerSystem _villagerSystem;
    private Villager _curVillager;
    private Villager _targetVillager;
    private Villager _prevTargetVillager;
    private Vector3 _targetPosition;

    private Transform _villageCenter;
    
    private DialogueSystem _dialogueSystem;
    
    private bool _isTalking;
    private bool _isFinishing;

    private float _lastChoosingTiming;

    public TalkRelaxVillagerState(NavmeshMovementAgent navmeshAgent, Villager villager, VillagerSystem villagerSystem, 
        DialogueSystem dialogueSystem, Transform villageCenter)
    {
        _navmeshAgent = navmeshAgent;
        _curVillager = villager;
        _transformHandler = new VillagerTransformHandler(_navmeshAgent);
        
        _villagerSystem = villagerSystem;
        _dialogueSystem = dialogueSystem;
        
        _villageCenter = villageCenter;
    }
    
    public override void EnterState()
    {
        ChooseTarget();
    }

    public override void EnterStateWithSkip()
    {
        EnterState();
    }

    private void ChooseTarget()
    {
        _lastChoosingTiming = Time.time;
        _targetVillager = null;
        int triesCount = 0;

        while (_targetVillager == null || _targetVillager == _curVillager || _targetVillager == _prevTargetVillager
                || _targetVillager.VillagerData.IsReservedForTalk || !_targetVillager.VillagerData.IsCanTalk || _targetVillager.VillagerData.IsMoving
                || Vector3.Distance(_navmeshAgent.transform.position, _targetVillager.transform.position) > MinDistance)
        {
            if (triesCount >= MinVillagerTries)
            {
                _targetVillager = null;
                break;
            }
            
            _targetVillager = _villagerSystem.GetVillager(new ActivityType[] { ActivityType.Work, ActivityType.Relax });
            triesCount++;
        }
        
        Debug.Log($"{_curVillager} choose Talk Target {_targetVillager}");
        if (_targetVillager)
        {
            _targetPosition = _targetVillager.transform.position;
            _targetVillager.VillagerData.IsReservedForTalk = true;
            _curVillager.VillagerData.IsReservedForTalk = true;
        }
    }
    
    public void Update()
    {
        if (_targetVillager != null && !_isTalking)
        {
            if (_targetVillager.VillagerData.IsMoving || _targetPosition != _targetVillager.transform.position)
            {
                ReleaseTalkingParams();
                return;
            }
            
            var movePoint = _targetVillager.transform.position + _targetVillager.transform.forward * TalkDistance;
            Quaternion targetRotation = Quaternion.LookRotation(_targetVillager.transform.position - _navmeshAgent.transform.position);
            _transformHandler.ActivateMovementWithRotation(movePoint, targetRotation, callback: OnPointReached);
        }
        else if (_targetVillager == null)
        {
            if (Time.time - _lastChoosingTiming >= MinTryTiming)
            {
                ChooseTarget();   
            }
            else if (!_curVillager.VillagerData.IsMoving)
            {
                _transformHandler.ActivateMovementWithPosInCircle(_villageCenter, MaxRadius);
            }
        }
    }

    private void OnPointReached()
    {
        _isTalking = true;

        _curVillager.VillagerData.IsTalking = true;
        _targetVillager.VillagerData.IsTalking = true;
        
        Debug.Log($"{_curVillager} reached talk point!");
        _dialogueSystem.PlayDialogue(_curVillager, _targetVillager, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        _prevTargetVillager = _targetVillager;
        ReleaseTalkingParams();
        if (!_isFinishing) ChooseTarget();
    }

    private void ReleaseTalkingParams()
    {
        _isTalking = false;
        
        _targetVillager.VillagerData.IsTalking = false;
        _curVillager.VillagerData.IsTalking = false;
        _targetVillager.VillagerData.IsReservedForTalk = false;
        _curVillager.VillagerData.IsReservedForTalk = false;
        
        _targetVillager = null;
        _transformHandler.DeactivateMovement();
    }

    public async override UniTask ExitState(CancellationToken token)
    {
        _isFinishing =  true;
        if (_isTalking)
        {
            await UniTask.WaitWhile(() => _isTalking, cancellationToken: token);
        }
        else _transformHandler.DeactivateMovement();

        _prevTargetVillager = null;
        _targetVillager = null;
        _isFinishing = false;
    }
    
    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navmeshAgent.GetCancellationTokenOnDestroy());
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}