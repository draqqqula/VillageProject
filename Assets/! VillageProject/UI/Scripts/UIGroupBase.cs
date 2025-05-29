using System.Collections.Generic;
using UnityEngine;

public abstract class UIGroupBase : MonoBehaviour
{
    [SerializeField] private List<UIPriority> _elements;
    public IEnumerable<UIPriority> Elements => _elements;
}
