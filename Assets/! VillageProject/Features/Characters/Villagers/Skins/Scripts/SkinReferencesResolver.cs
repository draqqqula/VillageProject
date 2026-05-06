using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using KevinIglesias;
using R3;
using UnityEngine;
using Zenject;

public class SkinReferencesResolver : MonoBehaviour
{
    [field: SerializeField] public Animator Animator {get; private set;}
    public AnimatorHandler AnimatorHandler {get; private set;}

    [SerializeField] private AnimationWindow _idleWindow;
    
    [field: SerializeField] public SkinnedMeshRenderer[] MeshRenderers {get; private set;}
    [field: SerializeField] public GameObject[] Accessories {get; private set;}

    [Inject] 
    private void Construct(DiContainer container)
    {
        var idleListener = container.ResolveId<IAnimationWindowListener>(_idleWindow);
        AnimatorHandler = new AnimatorHandler(Animator, idleListener);
    }
}

public class AnimatorHandler
{
    public Animator Animator {get; private set;}
    private CancellationToken _destroyToken;
    
    private IAnimationWindowListener _idleWindowListener;
    
    public bool IsTransitioning {get; private set;}

    public AnimatorHandler(Animator animator, IAnimationWindowListener idleWindowListener)
    {
        Animator = animator;
        _destroyToken = animator.GetCancellationTokenOnDestroy();
        
        _idleWindowListener = idleWindowListener;
        _idleWindowListener.IsActive.Subscribe(OnIdleWindow).AddTo(animator.gameObject);
    }

    private void OnIdleWindow(bool isActive)
    {
        if (isActive) IsTransitioning = false;
    }

    public void SetBool(string name, bool value)
    {
        Animator.SetBool(name, value);
    }

    public void SetTrigger(string name)
    {
        Animator.SetTrigger(name);
    }
    
    public async UniTask TransitByBool(string name, bool value, CancellationToken token)
    {
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, _destroyToken);
        
        IsTransitioning = true;
        Animator.SetBool(name, value);
        
        await UniTask.WaitWhile(() => IsTransitioning, cancellationToken: linkedToken.Token);
    }

    public async UniTask TransitByTrigger(string name, CancellationToken token)
    {
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, _destroyToken);
        
        IsTransitioning = true;
        Animator.SetTrigger(name);
        
        await UniTask.WaitWhile(() => IsTransitioning, cancellationToken: linkedToken.Token);
    }
}