using System;
using System.Collections.Generic;
using UnityEngine;
using ObservableCollections;
using System.Linq;
using System.Collections;
using Zenject;


public class MatchObjective : MonoBehaviour
{
    private ObservableList<float> _targets = new ObservableList<float>();
    [Inject] private MatchState _matchState;
    public IReadOnlyObservableList<float> Targets => _targets;
    
    public event Action OnEnemiesInVillage;

    public void Initialize(params float[] health)
    {
        for (int i = 0; i < health.Length; i++)
        {
            _targets.Add(health[i]);
        }
    }

    public void Take(TargetDamage damage)
    {
        OnEnemiesInVillage?.Invoke();
        damage.Deal(_targets);
        if (_targets.All(x => x == 0))
        {
            _matchState.DeclareDefeat();
        }
    }
}
