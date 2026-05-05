using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class CharacterHealthInstaller : MonoInstaller
{
    [SerializeField] private Health _health;
    [SerializeField] private DeathEvent _deathEvent;

    [SerializeField] private float _curHealth;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<Health>().FromInstance(_health).AsSingle();
        Container.BindInstance(_deathEvent).AsSingle();
        
        _health.AmountReactive.Subscribe(v => _curHealth = v).AddTo(this);
    }
}