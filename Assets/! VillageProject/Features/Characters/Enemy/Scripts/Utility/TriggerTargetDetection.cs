using UnityEngine;

public class TriggerTargetDetection : MonoBehaviour
{
    [SerializeField] private Target _target;

    private void OnTriggerEnter(Collider other)
    {
        var searcher = other.GetComponent<SearchForTarget>();
        if (searcher != null)
        {
            searcher.Detect(_target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var searcher = other.GetComponent<SearchForTarget>();
        if (searcher != null)
        {
            searcher.Forget(_target);
        }
    }
}
