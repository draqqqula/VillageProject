using System;
using System.Collections;
using UnityEngine;

public class DamageInteractionContext
{
    public DamageInteractionContext(IServiceProvider targetAttributes, IServiceProvider sourceAttributes)
    {
        TargetAttributes = targetAttributes;
        SourceAttributes = sourceAttributes;
    }

    public IServiceProvider TargetAttributes { get; private set; }
    public IServiceProvider SourceAttributes { get; private set; }
}