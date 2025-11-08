using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class AnimationWindowHandler : IAnimationWindowController, IAnimationWindowListener
{
    private ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(false);
    private AnimationWindow _window;
    public event Action OnEnter;
    public event Action OnExit;

    public AnimationWindowHandler(AnimationWindow window)
    {
        _window = window;
    }

    public float Progress { get; set; } = 0f;
    public bool Active
    {
        get
        {
            return _isActive.Value;
        }

        set
        {
            _isActive.Value = value;
            if (value)
            {
                OnEnter?.Invoke();
            }
            else
            {
                OnExit?.Invoke();
            }
        }
    }

    public ReadOnlyReactiveProperty<bool> IsActive => _isActive;
}
