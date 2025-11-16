using System.Collections;
using UnityEngine;

public class ShowAttackDirectionSignal
{
    public ShowAttackDirectionSignal(bool visible)
    {
        Visible = visible;
    }

    public bool Visible {  get; private set; }
}