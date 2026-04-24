using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TalkRelaxVillagerState : RelaxVillagerState, IUpdatableState
{
    private const float MinDistance = 20f;
    private const int MinVillagerTries = 7;
    private const float TalkDistance = 2f;
    private const float MinTryTiming = 5;
    
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerTransformHandler _transformHandler;
    
    private VillagerSystem _villagerSystem;
    private Villager _curVillager;
    private Villager _targetVillager;
    
    private DialogueSystem _dialogueSystem;
    
    private bool _isTalking;
    private bool _isFinishing;

    private float _lastChoosingTiming;

    public TalkRelaxVillagerState(NavmeshMovementAgent navmeshAgent, Villager villager, VillagerSystem villagerSystem, 
        DialogueSystem dialogueSystem)
    {
        _navmeshAgent = navmeshAgent;
        _curVillager = villager;
        _transformHandler = new VillagerTransformHandler(_navmeshAgent);
        
        _villagerSystem = villagerSystem;
        _dialogueSystem = dialogueSystem;
    }
    
    public override void EnterState()
    {
        ChooseTarget();
    }
    
    private void ChooseTarget()
    {
        _lastChoosingTiming = Time.time;
        _targetVillager = null;
        int triesCount = 0;

        while (_targetVillager == null || _targetVillager == _curVillager || _targetVillager.VillagerData.IsTalking || _targetVillager.VillagerData.IsMoving
               || Vector3.Distance(_navmeshAgent.transform.position, _targetVillager.transform.position) > MinDistance)
        {
            if (triesCount >= MinVillagerTries) break;
            
            _targetVillager = _villagerSystem.GetVillager(new ActivityType[] { ActivityType.Work, ActivityType.Relax });
            triesCount++;
        }
        
        if (_targetVillager) _targetVillager.VillagerData.IsTalking = true;
    }
    
    public void Update()
    {
        if (_targetVillager && !_isTalking)
        {
            if (_targetVillager.VillagerData.IsMoving) OnDialogueFinished();
            
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
        }
    }

    private void OnPointReached()
    {
        _isTalking = true;

        _curVillager.VillagerData.IsTalking = true;
        _dialogueSystem.PlayDialogue(_curVillager, _targetVillager, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        _isTalking = false;
        
        _targetVillager.VillagerData.IsTalking = false;
        _curVillager.VillagerData.IsTalking = false;
        if (!_isFinishing) ChooseTarget();
    }

    public async override UniTask ExitState(CancellationToken token)
    {
        _isFinishing =  true;
        if (_isTalking)
        {
            await UniTask.WaitWhile(() => _isTalking, cancellationToken: token);
        }
        
        _transformHandler.DeactivateMovement();
        _targetVillager.VillagerData.IsTalking = false;
        _curVillager.VillagerData.IsTalking = false;
        
        _targetVillager = null;
        _isFinishing = false;
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}