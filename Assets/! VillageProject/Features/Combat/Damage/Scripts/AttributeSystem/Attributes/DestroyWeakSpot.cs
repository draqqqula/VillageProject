using System;
using System.Collections;
using UnityEngine;
using Zenject;

[GenerateDamageAttribute(inject: true)]
public class DestroyWeakSpot : IDamageExecutable
{
    [Inject] private WeakSpotController _weakSpotController;
    public int GetPriority()
    {
        return 0;
    }

    public bool TryExecute(DamageInteractionContext context)
    {
        _weakSpotController.Close();
        return true;
    }
}