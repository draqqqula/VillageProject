using System.Collections;
using UnityEngine;

public class CompassOrigin : MonoBehaviour
{
    [SerializeField] private CompassDisplay _compass;
    [SerializeField] private RectTransform _display;
    [SerializeField] private RectTransform _canvas;

    private void Update()
    {
        _compass.ViewAngle = Camera.main.transform.rotation.eulerAngles.y;
        _compass.ViewPort = Camera.main.fieldOfView * (_display.rect.width / _canvas.rect.width);
    }

    public Vector3 GetDeltaToOrigin(Vector3 position)
    {
        return position - Camera.main.transform.position;
    }

    public float GetAngleTo(Vector3 position)
    {
        var signed = -Vector2.SignedAngle(Vector2.up, GetDeltaToOrigin(position).ToXZ());
        if (signed < 0)
        {
            return 360 + signed;
        }
        return signed;
    }
}