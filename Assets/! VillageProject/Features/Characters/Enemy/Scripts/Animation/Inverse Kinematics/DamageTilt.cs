using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DamageTilt : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint _constraint;
    [SerializeField] private Transform _target;
    [SerializeField] private AnimationCurve _weightOverTime;
    [SerializeField] private float _targetRange;

    private void Reset()
    {
        _constraint = GetComponent<MultiAimConstraint>();
        if (_constraint.data.sourceObjects.Count > 0)
        {
            _target = _constraint.data.sourceObjects[0].transform;
        }
    }

    public void Create(Vector3 direction)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateTilt(direction));
    }

    private IEnumerator AnimateTilt(Vector3 direction)
    {
        _target.transform.localPosition = direction * _targetRange;

        var t = _weightOverTime[0].time;
        var finish = _weightOverTime[_weightOverTime.length - 1].time;

        while (t < finish)
        {
            _constraint.weight = _weightOverTime.Evaluate(t);
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
        }

        _constraint.weight = 0;

        _target.transform.localPosition = Vector3.zero;
    }

    [ContextMenu("Test1 left")]
    private void Test1()
    {
        Create(Vector3.left);
    }

    [ContextMenu("Test2 right")]
    private void Test2()
    {
        Create(Vector3.right);
    }

    [ContextMenu("Test3 fwd")]
    private void Test3()
    {
        Create(Vector3.forward);
    }

    [ContextMenu("Test4 back")]
    private void Test4()
    {
        Create(Vector3.back);
    }
}
