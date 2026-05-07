using System;
using System.Collections;

public abstract class GuardVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Guard;
    public override void EnterStateWithSkip() { }
    public override void ExitStateWithSkip() { }
}