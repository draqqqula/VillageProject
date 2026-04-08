using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class MatchState : MonoBehaviour
{
    public enum MatchStateValue
    {
        Ongoing,
        Victory,
        Defeat
    }

    public UnityEvent OnVictory;
    public UnityEvent OnDefeat;
    public MatchStateValue State { get; private set; } = MatchStateValue.Ongoing;

    public void DeclareVictory()
    {
        if (IsOngoing())
        {
            State = MatchStateValue.Victory;
            OnVictory?.Invoke();
        }
    }

    public void DeclareDefeat()
    {
        if (IsOngoing())
        {
            State = MatchStateValue.Defeat;
            OnDefeat?.Invoke();
        }
    }

    private bool IsOngoing()
    {
        return State == MatchStateValue.Ongoing;
    }
}
