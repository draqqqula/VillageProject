using System;
using System.Collections;
using UnityEngine;

public class AnchorElement : MonoBehaviour
{
    [Flags]
    public enum Mode
    {
        View = 1 << 0,
        Hover = 1 << 1,
        Select = 1 << 2
    }

    [field: SerializeField] [field: EnumButtons] public Mode ActiveOn { get; private set; }
}