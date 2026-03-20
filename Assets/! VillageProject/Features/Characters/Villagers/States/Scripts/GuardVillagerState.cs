using System;
using System.Collections;

public abstract class GuardVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Guard;
}