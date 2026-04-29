using UnityEngine;

public class ScheduleAPITester : MonoBehaviour
{
    [SerializeField] private int _newStartPeriod;
    [SerializeField] private int _newEndPeriod;
    [SerializeField] private int _newLength;

    [SerializeField] private string _villagerKey;
    [SerializeField] private int _changingPeriod;
    
    [SerializeField] private ScheduleController _scheduleController;

    [ContextMenu("ChangeStartPeriod")]
    public void ChangeStartPeriod()
    {
        _scheduleController.ChangeStartForPeriod(_villagerKey, _changingPeriod, _newStartPeriod);
    }

    [ContextMenu("ChangeEndPeriod")]
    public void ChangeEndPeriod()
    {
        _scheduleController.ChangeEndForPeriod(_villagerKey, _changingPeriod, _newEndPeriod);
    }
    
    [ContextMenu("ChangeLengthPeriod")]
    public void ChangeLengthPeriod()
    {
        _scheduleController.ChangeLengthEvenlyForPeriod(_villagerKey, _changingPeriod, _newLength);
    }
}