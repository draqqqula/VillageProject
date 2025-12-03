using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AudioPlayer<T> : IInitializable, IDisposable where T : Enum
{
    [Inject] private Dictionary<T, AudioClip> _clips;
    [Inject] private SignalBus _signalBus;
    [Inject] private AudioSource _source;

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayAudioSignal<T>>(HandleSignal);
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayAudioSignal<T>>(HandleSignal);
    }

    private void HandleSignal(PlayAudioSignal<T> signal)
    {
        if (_clips.TryGetValue(signal.Key, out var clip))
        {
            _source.PlayOneShot(clip);
        }
    }

    public static void InstallTo(DiContainer container)
    {
        container.DeclareSignal<PlayAudioSignal<T>>();
        container.BindInterfacesAndSelfTo<AudioPlayer<T>>().AsSingle();
    }
}