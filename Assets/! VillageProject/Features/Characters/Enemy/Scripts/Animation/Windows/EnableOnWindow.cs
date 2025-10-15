using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EnableOnWindow : MonoBehaviour
{
    [SerializeField] private AnimationWindow _window;
    private IAnimationWindowListener _listener;
    [SerializeField] private GameObject _object;

    [Inject]
    private void Construct(DiContainer container)
    {
        _listener = container.ResolveId<IAnimationWindowListener>(_window);
    }

    private void Awake()
    {
        _listener.IsActive.Subscribe(HandleEntered).AddTo(this);
    }

    private void HandleEntered(bool value)
    {
        if (value)
        {
            _object.SetActive(true);
        }
        else
        {
            _object.SetActive(false);
        }
    }
}
