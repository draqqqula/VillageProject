using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Zenject;

public class AnimationWindowActivatorState : StateMachineBehaviour
{
    [Serializable]
    public class AnimationWindowInfo
    {
        [field: SerializeField] public AnimationWindow Window { get; private set; }
        [field: SerializeField] public float EnterAt { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        public float ExitAt => EnterAt + Duration;
    }

    class AnimationWindowState
    {
        public AnimationWindowState(
            AnimationWindow window,
            IAnimationWindowController listener,
            float startNormalized,
            float exitNormalized) 
        {
            Window = window;
            _isActive = false;
            _listener = listener;
            StartNormalized = startNormalized;
            ExitNormalized = exitNormalized;
        }

        private readonly IAnimationWindowController _listener;
        private bool _isActive;
        public readonly AnimationWindow Window;
        public readonly float StartNormalized;
        public readonly float ExitNormalized;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive == value)
                {
                    return;
                }
                _listener.Active = value;
                _isActive = value;
            }
        }

        public float Progress
        {
            get
            {
                return _listener.Progress;
            }
            set
            {
                _listener.Progress = value;
            }
        }
    }

    [SerializeField] private List<AnimationWindowInfo> _windows;
    private List<(IAnimationWindowController, AnimationWindow)> _listeners;

    [Inject]
    public void Construct(DiContainer container)
    {
        _listeners = new List<(IAnimationWindowController, AnimationWindow)>();
        foreach (var window in _windows)
        {
            var listener = container.TryResolveId<IAnimationWindowController>(window.Window);
            if (listener != null)
            {
                _listeners.Add((listener, window.Window));
            }
        }
    }

    private AnimationWindowState[] _states;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var state in _states)
        {
            state.IsActive = stateInfo.normalizedTime >= state.StartNormalized 
                && stateInfo.normalizedTime < state.ExitNormalized;
            if (state.IsActive)
            {
                state.Progress = Mathf.Clamp01(stateInfo.normalizedTime - state.StartNormalized);
            }
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        

        if (_states == null)
        {
            _states = _windows.Join(_listeners, it => it.Window, it => it.Item2, (info, listener) =>
            {
                return new AnimationWindowState(
                    info.Window,
                    listener.Item1,
                    info.EnterAt * stateInfo.length,
                    info.ExitAt * stateInfo.length);
            }).ToArray();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        foreach (var state in _states)
        {
            state.IsActive = false;
        }
    }
}