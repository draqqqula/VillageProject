using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(
    fileName = "New Damage Attributes",
    menuName = "Damage Attributes",
    order = 99)]
public class DamageAttributes : MonoBehaviour
{
    [SerializeReference, SubclassSelector] private List<DamageAttributeBase> Attributes;

    public void AddServices(IServiceCollection services)
    {
        foreach (var attribute in Attributes)
        {
            attribute.AddServices(services);
        }
    }
}