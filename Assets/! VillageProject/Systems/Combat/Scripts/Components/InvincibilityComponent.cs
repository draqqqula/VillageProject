using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class InvincibilityComponent : DamageComponentBase
{
    private Dictionary<object, float> _expireStamps = new Dictionary<object, float>();

    public float DefaultDuration;

    public void SetKey(object key, float durationFactor)
    {
        _expireStamps[key] = Time.time + DefaultDuration * durationFactor;
    }

    public bool IsKeyActive(object key)
    {
        if (_expireStamps.TryGetValue(key, out var expiration))
        {
            return Time.time <= expiration;
        }
        return false;
    }
}
