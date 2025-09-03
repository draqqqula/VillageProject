using BezierSolution;
using UnityEngine;

public class FenceBuilder : MonoBehaviour
{
    [SerializeField] private float _interval;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private BezierSpline _curve;
}
