using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class SkinReferencesResolver : MonoBehaviour
{
    [field: SerializeField] public Animator Animator {get; private set;}
    public AnimatorHandler AnimatorHandler {get; private set;}

    [Inject] 
    private void Construct([Inject(Id = "Transition")]IAnimationWindowListener windowListener)
    {
        AnimatorHandler = new AnimatorHandler(Animator, windowListener);
    }
}

public class AnimatorHandler
{
    public Animator Animator {get; private set;}
    private CancellationToken _destroyToken;
    private IAnimationWindowListener _transitionWindowListener;
    
    public bool IsTransitioning {get; private set;}

    public AnimatorHandler(Animator animator, IAnimationWindowListener transitionWindowListener)
    {
        Animator = animator;
        _destroyToken = animator.GetCancellationTokenOnDestroy();
        
        _transitionWindowListener = transitionWindowListener;
        _transitionWindowListener.IsActive.Subscribe(OnTransitionWindow).AddTo(animator.gameObject);
    }

    private void OnTransitionWindow(bool isActive)
    {
        IsTransitioning = isActive;
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
        
        Animator.SetBool(name, value);
        
        await UniTask.Yield();
        await UniTask.WaitWhile(() => IsTransitioning, cancellationToken: linkedToken.Token);
    }

    public async UniTask TransitByTrigger(string name, CancellationToken token)
    {
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, _destroyToken);
        
        Animator.SetTrigger(name);
        
        await UniTask.Yield();
        await UniTask.WaitWhile(() => IsTransitioning, cancellationToken: linkedToken.Token);
    }
}