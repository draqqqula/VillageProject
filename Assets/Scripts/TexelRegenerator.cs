using System.Collections;
using UnityEngine;

public class TexelRegenerator : MonoBehaviour
{
    [SerializeField] TexelSplatRaycast _raycast;
    [SerializeField] float _gridScale = 1f;
    [SerializeField] Vector3 _offset;

    private Vector3 _cached;

    private void Update()
    {
        var scaled = transform.position * _gridScale;

        var rounded = new Vector3(
            Mathf.Round(scaled.x),
            Mathf.Round(scaled.y),
            Mathf.Round(scaled.z)
        ) + _offset;

        var probePosition = rounded / _gridScale;

        //float alpha = CalculateAlpha(probePosition);

        //_raycast.SetAlpha(alpha);

        if (probePosition != _cached)
        {
            UpdateRaycast(probePosition);
            _cached = probePosition;
        }
    }

    private void UpdateRaycast(Vector3 probePosition)
    {
        _raycast.transform.position = probePosition;

        _raycast.ClearTexels();
        _raycast.GenerateTexels();
    }

    private float CalculateAlpha(Vector3 probePosition)
    {
        // расстояние от центра ячейки
        var distance = transform.position - probePosition;
        var dx = Mathf.Abs(distance.x);
        var dy = Mathf.Abs(distance.y);
        // максимальное расстояние — половина диагонали ячейки
        float cellSize = 1f / _gridScale;
        float maxDistance = cellSize;
        var ax = dx / maxDistance;
        var ay = dy / maxDistance;
        var best = Mathf.Max(ax, ay);

        // нормализация
        float alpha = 1f - Mathf.Clamp01(best);
        return alpha;
    }
}