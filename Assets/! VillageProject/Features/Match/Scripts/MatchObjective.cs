using System;
using System.Collections.Generic;
using UnityEngine;
using ObservableCollections;
using System.Linq;
using System.Collections;
using R3;
using Zenject;


public class MatchObjective : MonoBehaviour
{
    private ObservableList<float> _targets = new ObservableList<float>();
    [Inject] private MatchState _matchState;
    [Inject] private WaveController _waveController;
    public IReadOnlyObservableList<float> Targets => _targets;
    public bool IsEnemiesInVillage {get; private set;}
    
    public event Action OnEnemiesInVillage;

    public void Initialize(params float[] health)
    {
        for (int i = 0; i < health.Length; i++)
        {
            _targets.Add(health[i]);
        }

        _waveController.IsOnBreak.Subscribe(OnChangeWaveState).AddTo(this);
    }

    public void Take(TargetDamage damage)
    {
        if (!IsEnemiesInVillage)
        {
            IsEnemiesInVillage = true;
            OnEnemiesInVillage?.Invoke();
        }

        damage.Deal(_targets);
        if (_targets.All(x => x == 0))
        {
            _matchState.DeclareDefeat();
        }
    }

    private void OnChangeWaveState(bool isOnBreak)
    {
        if (isOnBreak) IsEnemiesInVillage = false;
    }
}
