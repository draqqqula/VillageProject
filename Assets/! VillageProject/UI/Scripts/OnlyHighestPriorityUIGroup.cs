using R3;
using R3.Triggers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnlyHighestPriorityUIGroup : UIGroupBase
{
    private HashSet<UIPriority> _overshadow = new HashSet<UIPriority>();
    private void Start()
    {
        foreach (var element in Elements)
        {
            SubscribeElement(element);
        }
    }

    private void SubscribeElement(UIPriority element)
    {
        element.OnEnableAsObservable()
            .Subscribe(_ => HandleEnabled(element))
            .AddTo(this);
        element.OnDisableAsObservable()
            .Subscribe(_ => HandleDisabled(element))
            .AddTo(this);
    }

    private void HandleEnabled(UIPriority element)
    {
        if (Elements.Any(it => it.Priority > element.Priority && it.gameObject.activeInHierarchy))
        {
            Disable(element);
            return;
        }
        foreach (var other in Elements.ToArray())
        {
            if (other.Priority < element.Priority)
            {
                Disable(other);
            }
        }
    }

    private void HandleDisabled(UIPriority element)
    {
        if (Elements.Any(it => it.Priority >= element.Priority && it.gameObject.activeInHierarchy))
        {
            return;
        }
        foreach (var overshadowed in _overshadow.ToArray())
        {
            if (overshadowed.Priority < element.Priority)
            {
                Enable(overshadowed);
            }
        }
    }

    private void Disable(UIPriority element)
    {
        if (element.gameObject.activeSelf)
        {
            _overshadow.Add(element);
            element.gameObject.SetActive(false);
        }
    }

    private void Enable(UIPriority element)
    {
        _overshadow.Remove(element);
        element.gameObject.SetActive(true);
    }
}
