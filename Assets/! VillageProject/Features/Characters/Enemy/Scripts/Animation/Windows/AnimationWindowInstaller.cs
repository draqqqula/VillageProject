using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AnimationWindowInstaller : MonoInstaller
{
    [SerializeField] private List<AnimationWindow> _animationWindows;

    public override void InstallBindings()
    {
        foreach (var window in _animationWindows)
        {
            Container.DeclareAnimationWindow(window);
        }
    }
}