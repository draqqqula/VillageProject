using System.Collections;
using UnityEngine;
using Microsoft.Extensions.DependencyInjection;

[GenerateDamageAttribute(ServiceLifetime.Scoped, true)]
public class TestAttribute : IDamageExecutable
{
    [FromDamageAttributeProperty] public string SomeString;
    [FromDamageAttributeProperty] public GameObject Prefab;
    [FromDamageAttributeProperty] public int SomeInt;
    private int _hitCounter = 0;

    public TestAttribute()
    {
        Debug.Log("Test attribute created");
    }
    public int GetPriority()
    {
        return 0;
    }

    public bool TryExecute(DamageInteractionContext context)
    {
        Debug.Log($"working! {SomeString} {Prefab.name} {_hitCounter}");
        _hitCounter += 1;
        return true;
    }
}