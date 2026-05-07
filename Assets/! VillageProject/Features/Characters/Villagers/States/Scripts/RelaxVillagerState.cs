public abstract class RelaxVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Relax;
    
    public override void EnterStateWithSkip() { }
    public override void ExitStateWithSkip() { }
}