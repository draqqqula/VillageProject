using R3;
using UnityEngine;
using Zenject;

public class EnableOnTwoWindows : MonoBehaviour
{
    [SerializeField] private AnimationWindow _enableWindow;
    [SerializeField] private AnimationWindow _disableWindow;
    
    private IAnimationWindowListener _enableListener;
    private IAnimationWindowListener _disableListener;
    
    
    [SerializeField] private GameObject _object;

    [Inject]
    private void Construct(DiContainer container)
    {
        _enableListener = container.ResolveId<IAnimationWindowListener>(_enableWindow);
        _disableListener = container.ResolveId<IAnimationWindowListener>(_disableWindow);
    }

    private void Awake()
    {
        _enableListener.IsActive.Subscribe(OnEnableWindowEntered).AddTo(this);
        _disableListener.IsActive.Subscribe(OnDisableWindowEntered).AddTo(this);
    }

    private void OnEnableWindowEntered(bool value)
    {
        if (value)
        {
            _object.SetActive(true);
        }
    }

    private void OnDisableWindowEntered(bool value)
    {
        if (value)
        {
            _object.SetActive(false);   
        }
    }
}