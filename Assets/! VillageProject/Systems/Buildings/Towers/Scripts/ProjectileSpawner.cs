using System;
using System.Collections;
using UnityEngine;

public abstract class ProjectileSpawner
{
    public abstract float Spawn(Transform target, Transform origin);
}