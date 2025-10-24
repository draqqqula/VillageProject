using System.Collections;
using UnityEngine;

public interface IAnimationWindowController
{
    public float Progress { get; set; }
    public bool Active { get; set; }
}