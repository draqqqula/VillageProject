using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

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
            AnimationWindowListener listener,
            float startNormalized,
            float exitNoralized) 
        {
            Window = window;
            _isActive = false;
            _listener = listener;
            StartNormalized = startNormalized;
            ExitNormalized = exitNoralized;
        }

        private readonly AnimationWindowListener _listener;
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
                _listener.enabled = value;
                _isActive = value;
            }
        }
    }

    [SerializeField] private List<AnimationWindowInfo> _windows;

    private AnimationWindowState[] _states;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var state in _states)
        {
            state.IsActive = stateInfo.normalizedTime >= state.StartNormalized 
                && stateInfo.normalizedTime < state.ExitNormalized;
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        var listeners = animator.GetComponents<AnimationWindowListener>();

        if (_states == null)
        {
            _states = _windows.Join(listeners, it => it.Window, it => it.Window, (info, listener) =>
            {
                return new AnimationWindowState(
                    info.Window,
                    listener,
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