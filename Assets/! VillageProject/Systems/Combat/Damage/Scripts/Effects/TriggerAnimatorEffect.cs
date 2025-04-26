using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class TriggerAnimatorEffect : DamageEffectBase
{
    [SerializeField] private Animator Animator;
    [SerializeField] private string Trigger;

    public override float Apply(DamageContext context, float baseDamage)
    {
        Animator.SetTrigger(Trigger);
        return baseDamage;
    }
}