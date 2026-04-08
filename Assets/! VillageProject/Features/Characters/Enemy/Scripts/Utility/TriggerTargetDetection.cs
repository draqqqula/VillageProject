using UnityEngine;

public class TriggerTargetDetection : MonoBehaviour
{
    [SerializeField] private Target _target;
    [SerializeField] private TeamMember _teamMember;

    public void SetTarget(Target target)
    {
        if (_target == null) _target = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }
        
        var searcher = other.GetComponent<SearchForTarget>();
        if (searcher != null && searcher.TeamMember.Team.IsEnemiesWith(_teamMember.Team))
        {
            searcher.Detect(_target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }
        
        var searcher = other.GetComponent<SearchForTarget>();
        if (searcher != null)
        {
            searcher.Forget(_target);
        }
    }
}