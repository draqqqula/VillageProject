using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ZoneTracker : MonoBehaviour
{
    private const string ZoneTag = "zone";
    [SerializeField] private GameObject _activeZone;
    private SortedList<int, GameObject> _zones = new SortedList<int, GameObject>();

    public UnityEvent<GameObject> OnNewActiveZone;

    public GameObject ActiveZone
    {
        get
        {
            return _activeZone;
        }
        private set
        {
            if (!ReferenceEquals(_activeZone, value))
            {
                _activeZone = value;
                OnNewActiveZone?.Invoke(value);
            }
        }
    }
    public IEnumerable<GameObject> AllZones => _zones.Values;

    public bool IsIn(int zoneOrder)
    {
        return _zones.ContainsKey(zoneOrder);
    }

    public bool IsActive(int zoneOrder)
    {
        return _activeZone != null && GetOrder(_activeZone) == zoneOrder;
    }

    public bool IsIn(GameObject zone)
    {
        return IsIn(GetOrder(zone));
    }

    public bool IsActive(GameObject zone)
    {
        return IsActive(GetOrder(zone));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ZoneTag))
        {
            var order = GetOrder(other.gameObject);
            _zones.TryAdd(order, other.gameObject);

            try
            {
                if (IsHighestPriority(order))
                {
                    ActiveZone = other.gameObject;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ZoneTag))
        {
            var order = GetOrder(other.gameObject);
            if (IsHighestPriority(order))
            {
                if (_zones.Count > 1)
                {
                    ActiveZone = _zones.Values[1];
                }
                else
                {
                    ActiveZone = null;
                }
            }
            _zones.Remove(GetOrder(other.gameObject));
        }
    }

    private string GetZoneName(GameObject zone)
    {
        return zone.name;
    }

    private int GetOrder(GameObject zone)
    {
        return zone.transform.GetSiblingIndex();
    }

    private bool IsHighestPriority(int order)
    {
        return _zones.IndexOfKey(order) == 0;
    }
}
