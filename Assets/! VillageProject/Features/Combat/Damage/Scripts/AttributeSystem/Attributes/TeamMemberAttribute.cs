using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class TeamMemberAttribute : DamageAttributeBase
{
    public class Data : ITeamAttribute
    {
        [Inject] public TeamMember TeamMember { get; private set; }
        public Team Team => TeamMember.Team;
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingletonInjectable<ITeamAttribute, Data>();
    }
}