using BezierSolution;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class FencePlacer : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private BezierSpline _spline;
    [SerializeField] private List<GameObject> _prefabs;
    [SerializeField] private float _distance;
    [SerializeField] protected LayerMask _layerMask;
    [SerializeField] private float _raycastDistance;
    [SerializeField] private float _wallHeight;

    [ContextMenu("Place")]
    private void Place()
    {
        var positionArray = GetPositions().ToArray();
        var combine = new CombineInstance[positionArray.Length];

        for (int i = 0; i < positionArray.Length; i++)
        {
            var prefab = GetRandomElement();
            var obj = Instantiate(prefab, transform);
            obj.transform.position = positionArray[i] - transform.position;
            var meshFilter = obj.GetComponent<MeshFilter>();
            combine[i].mesh = meshFilter.sharedMesh;
            combine[i].transform = meshFilter.transform.localToWorldMatrix;
            DestroyImmediate(obj);
        }
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        _meshFilter.sharedMesh = mesh;
    }

    private GameObject GetRandomElement()
    {
        return _prefabs[Random.Range(0, _prefabs.Count)];
    }

    private IEnumerable<Vector3> GetPositions()
    {
        for (float distance = 0; distance < _spline.evenlySpacedPoints.splineLength; distance += _distance)
        {
            var t = _spline.evenlySpacedPoints.GetNormalizedTAtDistance(distance);
            var pointOnSpline = _spline.GetPoint(t);
            var raycast = Physics.Raycast(pointOnSpline, Vector3.down, out var hitInfo, _raycastDistance, _layerMask);
            yield return hitInfo.point;
        }
    }
}
