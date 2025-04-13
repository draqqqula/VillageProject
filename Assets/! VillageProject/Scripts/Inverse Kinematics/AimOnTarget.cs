using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimOnTarget : MonoBehaviour
{
    [SerializeField] private SearchForTarget _searcher;
    [SerializeField] private MultiAimConstraint _constraint;

    void OnEnable()
    {
        _searcher.OnMainTargetChanged.AddListener(HandleTargetAssigned);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        _searcher.OnMainTargetChanged.RemoveListener(HandleTargetAssigned);
    }

    private void HandleTargetAssigned()
    {
        StopAllCoroutines();
        if (_searcher.MainTarget != null)
        {
            _constraint.weight = 1f;
            StartCoroutine(SetAim(_searcher.MainTarget.transform));
        }
        else
        {
            _constraint.weight = 0f;
        }
    }

    private IEnumerator SetAim(Transform target)
    {
        while (true)
        {
            transform.position = target.position;
            yield return new WaitForEndOfFrame();
        }
    }
}
