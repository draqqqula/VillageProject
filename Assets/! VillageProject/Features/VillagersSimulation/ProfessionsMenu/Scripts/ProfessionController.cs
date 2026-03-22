using UnityEngine;

public class ProfessionController : MonoBehaviour
{
    [SerializeField] private VillagerSystem _villagerSystem;
    [SerializeField] private ScheduleView _scheduleView;

    public void Init()
    {
        _scheduleView.OnProfessionChanged += InvokeProfessionChanged;
    }
    
    public void ChangeProfession(string villagerKey, ProfessionType professionType, bool isUpdateView = true)
    {
        var villager = _villagerSystem.GetVillager(villagerKey);
        if (villager != null) ChangeVillager(villager, professionType, isUpdateView);
    }

    public void ChangeVillager(Villager villager, ProfessionType professionType, bool isUpdateView = true)
    {
        if (villager.VillagerData.Profession.Type == professionType) return;
        
        villager.SwitchProfession(professionType);
        if (isUpdateView) _scheduleView.UpdateProfessionView(villager.VillagerData.Key, professionType);
    }
    
    private void InvokeProfessionChanged(string villagerKey, ProfessionType profession)
    {
        ChangeProfession(villagerKey, profession, false);
    }

    private void OnDestroy()
    {
        _scheduleView.OnProfessionChanged -= InvokeProfessionChanged;
    }
}