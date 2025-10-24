using System.Collections;
using UnityEngine;
using Zenject;


public static class AnimationWindowExtensions
{
    public static void DeclareAnimationWindow(this DiContainer container, AnimationWindow window)
    {
        var handler = new AnimationWindowHandler(window);
        container.Bind<IAnimationWindowListener>().WithId(window).FromInstance(handler).AsCached();
        container.Bind<IAnimationWindowController>().WithId(window).FromInstance(handler).AsCached();
        container.Bind<IAnimationWindowListener>().WithId(window.Key).FromInstance(handler).AsCached();
        container.Bind<IAnimationWindowController>().WithId(window.Key).FromInstance(handler).AsCached();
    }
}