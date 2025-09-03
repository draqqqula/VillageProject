using R3.Triggers;
using R3;
using System.Collections;
using UnityEngine;
using Zenject;

public class CompassMapPosition : MonoBehaviour
{
    [Inject] private CompassOrigin Origin;
    [SerializeField] private CompassTracker Tracker;

    private void Update()
    {
        if (Tracker.Marker != null)
        {
            Tracker.Marker.Angle = Origin.GetAngleTo(transform.position);
        }
    }
}