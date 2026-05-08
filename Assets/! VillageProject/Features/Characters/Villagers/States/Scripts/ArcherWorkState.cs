using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class ArcherWorkState : WorkVillagerState
{
    private VillagerTransformHandler _transformHandler;
    private NavmeshMovementAgent _navmeshAgent;
    private Profession _profession;
    private VillagerData _villagerData;
    
    private SkinReferencesResolver _skinReferencesResolver;
    private SearchForTarget _searchForTarget;
    private Collider _discoveryCollider;

    private Building _archerTower;
    private BuildingStorage _storage;
    private WaveController _waveController;
    
    private VillagerStateFactory _factory;
    private DefenderWorkState _patrulState;
    private Transform _villageCenter;
    
    private RaiseExperienceHandler _experienceHandler;
    private VillagerFadingHandler _fadingHandler;
    private GameTimer _gameTimer;
    
    private bool _isInited = false;
    private SkipTimeController _skipTimeController;
    
    public ArcherWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver, Profession profession,
        BuildingStorage buildingStorage, SearchForTarget searchForTarget, Collider discoveryCollider, VillagerStateFactory factory,
        GameTimer gameTimer, Transform villageCenter, WaveController waveController, SkipTimeController skipTimeController, 
        VillagerData villagerData)
    {
        _navmeshAgent = navmeshAgent;
        _profession = profession;
        _skinReferencesResolver = skinReferencesResolver;
        _villagerData = villagerData;
        
        _searchForTarget = searchForTarget;
        _discoveryCollider = discoveryCollider;
        
        _storage = buildingStorage;
        _factory = factory;
        _waveController = waveController;
        _villageCenter = villageCenter;
        
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _experienceHandler = new RaiseExperienceHandler(profession, gameTimer);
        _fadingHandler = new VillagerFadingHandler(skinReferencesResolver);
        _skipTimeController = skipTimeController;

        _waveController.OnWaveRoadChanged += OnRoadChanged;
        _profession.Experience.Subscribe(TryRiseAttack).AddTo(_navmeshAgent.gameObject);
        _isInited = true;
    }

    private void OnRoadChanged(string[] roadIndexes)
    {
        if (_waveController.IsWaveInNextNight) return;
        _transformHandler.DeactivateMovement();
        _ = ChangeTower(_navmeshAgent.GetCancellationTokenOnDestroy());
    }

    private async UniTask ChangeTower(CancellationToken token)
    {
        if (!_skipTimeController.IsSkipping.CurrentValue) await ExitTower(token);
        else _ = ExitTower(token);
        ChooseArcherTower(_waveController.GetWaveRoadIndexes());
    }

    public override void EnterState()
    {
        if (!_isInited) return;
        PlayStartActions(_waveController.GetWaveRoadIndexes());
    }

    public override void EnterStateWithSkip()
    {
        if (!_isInited) return;
        PlayStartActionsWithSkip(_waveController.GetWaveRoadIndexes());
    }
    
    public void PlayStartActions(string[] roadIndexes)
    {
        _archerTower = ChooseArcherTower(roadIndexes);

        if (_archerTower == null)
        {
            _patrulState = _factory.CreateDefenderWorkState();
            _patrulState.EnterState();
            return;
        }
        _transformHandler.ActivateMovementWithRotation(_archerTower.Data.EnterPoint, callback: OnPointReached);
    }
    
    public void PlayStartActionsWithSkip(string[] roadIndexes)
    {
        _archerTower = ChooseArcherTower(roadIndexes);
        if (_archerTower == null)
        {
            _patrulState = _factory.CreateDefenderWorkState();
            _patrulState.EnterStateWithSkip();
            return;
        }
        
        var distance = Vector3.Distance(_navmeshAgent.transform.position, _archerTower.Data.EnterPoint.position);
        var moveHours = (int)Mathf.Ceil(distance / _villagerData.SpeedInHour);
        
        _navmeshAgent.transform.position = _archerTower.Data.EnterPoint.position;
        _experienceHandler.IncreaseHours(moveHours);
        
        Debug.Log("Enter Archer Tower");
        _ = EnterTower(_navmeshAgent.GetCancellationTokenOnDestroy());
    }

    private Building ChooseArcherTower(string[] roadIndexes)
    {
        Building archerTower = null;
        var archerTowers = _storage.GetAll(BuildingType.ArcherTower, BuildingData.State.Wait, roadIndexes);
        
        if (archerTowers != null && archerTowers.Count > 0)
        {
            archerTower = archerTowers.OrderBy(a => Vector3.Distance(a.transform.position, _villageCenter.position)).FirstOrDefault();
        }
        
        if (archerTower == null)
        {
            Debug.LogWarning($"{_archerTower} is not valid ArcherTower!");
            return null;
        }
        
        archerTower.SetReserved();
        Debug.Log("Found ArcherTower!");
        return archerTower;
    }
    
    private void OnPointReached()
    {
        _fadingHandler.FadeOut(OnFadingEnded);
    }

    private void OnFadingEnded()
    {
        _ = EnterTower(_navmeshAgent.GetCancellationTokenOnDestroy());
    }

    private async UniTask EnterTower(CancellationToken token)
    {
        if (!_skipTimeController.IsSkipping.CurrentValue) await _fadingHandler.FadeOutAsync(token);
        
        var archerPoint = (_archerTower.Data as ArcherTowerData).ArcherPoint;
        _navmeshAgent.UnconnectFromNavmeshManually();
        _navmeshAgent.transform.position = archerPoint.position;
        _discoveryCollider.enabled = false;
        
        Debug.Log($"NavMesh enabled {_navmeshAgent.enabled}");
        
        if (!_skipTimeController.IsSkipping.CurrentValue) await _fadingHandler.FadeInAsync(token);
        
        (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked += OnShootInvoked;
        _archerTower.SetReady();
        
        _skinReferencesResolver.Animator.SetBool("Agressed", true);
        _skinReferencesResolver.Animator.SetBool("Work", true);
        
        _experienceHandler.StartRaisingExperience();
        TryRiseAttack(_profession.Experience.CurrentValue);
        _villagerData.IsOnTower = true;
    }
    
    private async UniTask ExitTower(CancellationToken token)
    {
        if (_fadingHandler.IsFading && !_skipTimeController.IsSkipping.CurrentValue) await _fadingHandler.WaitFading(token);
        if (_archerTower != null) _archerTower.SetWaiting();
        
        if (_villagerData.IsOnTower)
        {
            Debug.Log("Exit Archer Tower");
            _experienceHandler.StopRaisingExperience();
            (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked -= OnShootInvoked;
            
            _skinReferencesResolver.Animator.SetBool("Agressed", false);

            if (!_skipTimeController.IsSkipping.CurrentValue)
            {
                await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
                await _fadingHandler.FadeOutAsync(token);
            }
            else _skinReferencesResolver.AnimatorHandler.SetBool("Work", false);
            
            var point = _archerTower.Data.EnterPoint;
            _navmeshAgent.transform.position = point.position;
            _navmeshAgent.ConnectToNavmeshManually();
            
            if (!_skipTimeController.IsSkipping.CurrentValue) await _fadingHandler.FadeInAsync(token);
            _discoveryCollider.enabled = true;
        }
        
        _archerTower = null;
        _villagerData.IsOnTower = false;
        Debug.Log($"Is on Tower {_villagerData.IsOnTower}");
    }

    private void OnShootInvoked()
    {
        if (_searchForTarget.MainTarget == null) return;
        
        _transformHandler.ActivateRotation(_searchForTarget.MainTarget.transform.position);
        _skinReferencesResolver.Animator.SetTrigger("Attack");
    }

    private void TryRiseAttack(float experience)
    {
        if (!_villagerData.IsOnTower) return;
        
        var multiplier = (_profession.ProfessionData as ArcherProfessionData).DamageMultiplierCurve.Evaluate(experience);
        (_archerTower.Data as ArcherTowerData).DamageMultiplier = multiplier;
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        if (!_isInited) return;

        if (_patrulState != null)
        {
            await _patrulState.ExitState(token);
            _patrulState = null;
            return;
        }

        if (!_skipTimeController.IsSkipping.CurrentValue) await ExitTower(token);
        else _ = ExitTower(token);
        _transformHandler.DeactivateMovement();
        
        Debug.Log("Exited Archer work state!");
    }

    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navmeshAgent.GetCancellationTokenOnDestroy());
    }

    public override void Dispose()
    {
        if (!_isInited) return;
        
        if (_villagerData.IsOnTower) (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked -= OnShootInvoked;
        _waveController.OnWaveRoadChanged -= OnRoadChanged;
        _transformHandler.Dispose();
        _fadingHandler.Dispose();
    }
}