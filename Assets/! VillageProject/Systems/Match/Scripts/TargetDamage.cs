using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class TargetDamage : MonoBehaviour
{
    [field: SerializeField] public float Damage {  get; private set; }

    public virtual void Deal(IList<float> targets)
    {
        Deal(targets, Damage);
    }

    private void Deal(IList<float> targets, float amount)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] > 0)
            {
                var health = targets[i] - amount;
                if (health < 0)
                {
                    targets[i] = 0;
                    Deal(targets, -health);
                }
                else
                {
                    targets[i] = health;
                }
                return;
            }
        }
    }
}