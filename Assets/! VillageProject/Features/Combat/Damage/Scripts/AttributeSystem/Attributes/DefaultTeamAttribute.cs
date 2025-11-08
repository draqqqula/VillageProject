using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class DefaultTeamAttribute : DamageAttributeBase
{
    public class Data : ITeamAttribute
    {
        public Data(Team team)
        {
            Team = team;
        }

        public Team Team {  get; private set; }
    }

    [SerializeField] private Team _team;

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<ITeamAttribute>(new Data(_team));
    }
}