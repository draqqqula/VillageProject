using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using UnityEngine;
using Zenject;

[GenerateDamageAttribute]
public class AdrenalineAmount
{
    [FromDamageAttributeProperty] public float Amount;
    [FromDamageAttributeProperty] public float Cooldown;
}