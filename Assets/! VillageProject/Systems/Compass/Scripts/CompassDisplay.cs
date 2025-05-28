using System.Collections.Generic;
using UnityEngine;

public class CompassDisplay : MonoBehaviour
{
    [SerializeField] private CompassMarkerManager _markers;
    [SerializeField] private RectTransform _compass;
    [field: SerializeField] public float ViewPort { get; set; } = 45;
    [field: SerializeField] public float ViewAngle { get; set; }

    private float HorizontalWindow => _compass.rect.width / 2;

    private void LateUpdate()
    {
        foreach (CompassMarker marker in _markers.All)
        {
            var deltaAngle = Mathf.DeltaAngle(ViewAngle, marker.Angle);
            if (deltaAngle <= marker.ViewPort)
            {
                var fadeFactor = (Mathf.Abs(deltaAngle) - marker.ClearZone) / marker.FadeZone;
                if (fadeFactor >= 0)
                {
                    marker.SetOpacity(1 - fadeFactor);
                }
                else
                {
                    marker.SetOpacity(1);
                }
                var t = Mathf.Clamp(deltaAngle / ViewPort, -1, 1);
                marker.gameObject.SetActive(true);
                marker.transform.localPosition = new Vector3(HorizontalWindow * t, 0);
            }
            else
            {
                marker.gameObject.SetActive(false);
            }
        }
    }
}
