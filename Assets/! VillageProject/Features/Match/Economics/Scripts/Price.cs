using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Price
{
    [SerializeField] private List<ResourceAmount> _required;

    public IEnumerable<ResourceAmount> Required => _required;

    public bool IsAvailable()
    {
        return _required.All(cost => cost.Amount <= cost.Resource.Amount);
    }

    public IEnumerable<ResourceAmount> GetMissing()
    {
        foreach (var cost in _required)
        {
            if (cost.Amount > cost.Resource.Amount)
            {
                yield return new ResourceAmount(cost.Resource, cost.Amount - cost.Resource.Amount);
            }
        }
    }

    public bool TryPay()
    {
        if (!IsAvailable())
        {
            return false;
        }
        foreach (var cost in _required)
        {
            cost.Resource.Decrement(cost.Amount);
        }
        return true;
    }

    public bool TryPay(out IEnumerable<ResourceAmount> missing)
    {
        missing = GetMissing();
        if (missing.Any())
        {
            return false;
        }
        foreach (var cost in _required)
        {
            cost.Resource.Decrement(cost.Amount);
        }
        return true;
    }
}