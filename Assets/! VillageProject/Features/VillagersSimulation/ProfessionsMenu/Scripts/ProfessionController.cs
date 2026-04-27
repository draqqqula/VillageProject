using System;
using System.Linq;
using UnityEngine;

public class ProfessionController : MonoBehaviour
{
    [SerializeField] private ProfessionData[] _professions;
    
    [SerializeField] private VillagerSystem _villagerSystem;
    [SerializeField] private ScheduleView _scheduleView;

    public event Action<Villager, ProfessionType> OnProfessionChanged;
    
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
        
        var profession = _professions.FirstOrDefault(p => p.Type == professionType);

        if (profession == null)
        {
            Debug.LogError($"Can't find profession data with type {professionType}!");
            return;
        }
        
        var professionInstance = ScriptableObject.Instantiate(profession);
        villager.SwitchProfession(new Profession() {ProfessionData = professionInstance});
        OnProfessionChanged?.Invoke(villager, professionType);
        
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