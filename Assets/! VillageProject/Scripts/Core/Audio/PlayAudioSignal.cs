using System;
using System.Collections;
using UnityEngine;

public class PlayAudioSignal<T> where T : Enum
{
    public PlayAudioSignal(T key)
    {
        Key = key;
    }

    public T Key { get; private set; }
}