using System.Linq;
using UnityEngine;

public class GatesHighlight : MonoBehaviour
{
    private const string Highlight = "Highlight";

    private GameObject[] _objects;
    void Awake()
    {
        _objects = FindObjectsByType<GateState>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Select(it => it.transform.Find(Highlight).gameObject)
            .ToArray();
    }

    private void OnEnable()
    {
        foreach (var obj in _objects)
        {
            obj.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (var obj in _objects)
        {
            obj.SetActive(false); 
        }
    }
}
