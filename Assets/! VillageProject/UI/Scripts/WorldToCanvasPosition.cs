using UnityEngine;

public class WorldToCanvasPosition : MonoBehaviour
{
    [field: SerializeField] public Vector3 WorldPosition { get; set; }
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        var normalized = _canvas.worldCamera.WorldToViewportPoint(WorldPosition) - new Vector3(0.5f, 0.5f);
        transform.localPosition = new Vector3(normalized.x * _canvas.pixelRect.width,
            normalized.y * _canvas.pixelRect.height);
    }
}
