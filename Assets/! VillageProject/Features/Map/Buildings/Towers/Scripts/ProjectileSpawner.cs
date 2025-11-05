using System;
using System.Collections;
using UnityEngine;
using Zenject;

public abstract class ProjectileSpawner
{
    public abstract float Spawn(DiContainer container, Transform target, Transform origin);
}