using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Damage", 
    menuName = "Damage Info", 
    order = 99)]
public class DamageInfo : ScriptableObject
{
    [SerializeReference, SubclassSelector] public List<DamageComponentBase> Components;
    [SerializeReference, SubclassSelector] public List<DamageConditionBase> Conditions;
    [SerializeReference, SubclassSelector] public List<DamageEffectBase> Effects;
    [field: SerializeField] public float BaseAmount { get; private set; }

    public IDamageComponentProvider ComponentProvider { get; private set; }

    private void OnEnable()
    {
        ComponentProvider = new DefaultDamageComponentProvider(Components);
    }
}
