using System.Collections;
using UnityEngine;

public class SetMarkerOnEnterVillage : MonoBehaviour
{
    [SerializeField] private ZoneTracker _tracker;
    [SerializeField] private CompassTracker _compassTracker;
    [SerializeField] private CompassMapPosition _mapPosition;

    private void OnEnable()
    {
        _tracker.OnNewActiveZone.AddListener(HandleNewActiveZone);
    }

    private void HandleNewActiveZone(GameObject zone)
    {
        if (zone.name == "Village")
        {
            _compassTracker.enabled = true;
            _mapPosition.enabled = true;
        }
    }
}