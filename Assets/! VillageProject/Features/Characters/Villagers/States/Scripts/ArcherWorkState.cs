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
    
    private bool _isInited = false;
    private bool _isOnTower = false;
    
    public ArcherWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver, Profession profession,
        BuildingStorage buildingStorage, SearchForTarget searchForTarget, Collider discoveryCollider, VillagerStateFactory factory,
        GameTimer gameTimer, Transform villageCenter, WaveController waveController)
    {
        _navmeshAgent = navmeshAgent;
        _profession = profession;
        _skinReferencesResolver = skinReferencesResolver;
        
        _searchForTarget = searchForTarget;
        _discoveryCollider = discoveryCollider;
        
        _storage = buildingStorage;
        _factory = factory;
        _waveController = waveController;
        _villageCenter = villageCenter;
        
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _experienceHandler = new RaiseExperienceHandler(profession, gameTimer);
        _fadingHandler = new VillagerFadingHandler(skinReferencesResolver);

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
        await ExitTower(token);
        ChooseArcherTower(_waveController.GetWaveRoadIndexes());
    }
    
    public override void EnterState()
    {
        if (!_isInited) return;
        ChooseArcherTower(_waveController.GetWaveRoadIndexes());
    }

    private void ChooseArcherTower(string[] roadIndexes)
    {
        _archerTower = null;
        
        var archerTowers = _storage.GetAll(BuildingType.ArcherTower, BuildingData.State.Wait, roadIndexes);
        if (archerTowers != null && archerTowers.Count > 0)
        {
            _archerTower = archerTowers.OrderBy(a => Vector3.Distance(a.transform.position, _villageCenter.position)).FirstOrDefault();
        }
        
        if (_archerTower == null)
        {
            Debug.LogWarning($"{_archerTower} is not valid ArcherTower!");
            _patrulState = _factory.CreateDefenderWorkState();
            _patrulState.EnterState();
            return;
        }
        
        _archerTower.SetReserved();
        Debug.Log("Found ArcherTower!");
        _transformHandler.ActivateMovementWithRotation(_archerTower.Data.EnterPoint, callback: OnPointReached);
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
        var archerPoint = (_archerTower.Data as ArcherTowerData).ArcherPoint;
        _navmeshAgent.enabled = false;
        _navmeshAgent.transform.position = archerPoint.position;
        _discoveryCollider.enabled = false;
        
        await _fadingHandler.FadeInAsync(token);
        (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked += OnShootInvoked;
        _archerTower.SetReady();
        
        _skinReferencesResolver.Animator.SetBool("Agressed", true);
        _skinReferencesResolver.Animator.SetBool("Work", true);
        
        _experienceHandler.StartRaisingExperience();
        TryRiseAttack(_profession.Experience.CurrentValue);
        _isOnTower = true;
    }

    private async UniTask ExitTower(CancellationToken token)
    {
        if (_fadingHandler.IsFading) await _fadingHandler.WaitFading(token);
        if (_archerTower != null) _archerTower.SetWaiting();
        
        if (_isOnTower)
        {
            _experienceHandler.StopRaisingExperience();
            (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked -= OnShootInvoked;
            
            _skinReferencesResolver.Animator.SetBool("Agressed", false);
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
            await _fadingHandler.FadeOutAsync(token);
            
            var point = _archerTower.Data.EnterPoint;
            _navmeshAgent.transform.position = point.position;
            _navmeshAgent.enabled = true;
            
            await _fadingHandler.FadeInAsync(token);
            _discoveryCollider.enabled = true;
        }
        _archerTower = null;
    }

    private void OnShootInvoked()
    {
        if (_searchForTarget.MainTarget == null) return;
        
        _transformHandler.ActivateRotation(_searchForTarget.MainTarget.transform.position);
        _skinReferencesResolver.Animator.SetTrigger("Attack");
    }

    private void TryRiseAttack(float experience)
    {
        if (!_isOnTower) return;
        
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

        await ExitTower(token);
        _transformHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        if (!_isInited) return;
        
        if (_isOnTower) (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked -= OnShootInvoked;
        _waveController.OnWaveRoadChanged -= OnRoadChanged;
        _transformHandler.Dispose();
        _fadingHandler.Dispose();
    }
}