using System.Collections;
using UnityEngine;

public static class AnchorExtensions
{

    public static AnchorLinks.Direction Opposite(this AnchorLinks.Direction self)
    {
        switch (self)
        {
            case AnchorLinks.Direction.Left: return AnchorLinks.Direction.Right;
            case AnchorLinks.Direction.Right: return AnchorLinks.Direction.Left;
            case AnchorLinks.Direction.Up: return AnchorLinks.Direction.Down;
            case AnchorLinks.Direction.Down: return AnchorLinks.Direction.Up;
            default: return AnchorLinks.Direction.None;
        }
    }
}