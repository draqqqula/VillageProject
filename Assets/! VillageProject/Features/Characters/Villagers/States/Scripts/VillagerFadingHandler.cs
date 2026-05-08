using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class VillagerFadingHandler : IDisposable
{
    private SkinReferencesResolver _skinReferencesResolver;
    private Sequence _fadeSequence;
    
    public bool IsFading { get; private set; }

    public VillagerFadingHandler(SkinReferencesResolver skinReferencesResolver)
    {
        _skinReferencesResolver = skinReferencesResolver;
    }

    public async UniTask FadeOutAsync(CancellationToken token, Action callback = null)
    {
        FadeOut(callback);
        await WaitFading(token);
    }
    
    public async UniTask FadeInAsync(CancellationToken token, Action callback = null)
    {
        FadeIn(callback);
        await WaitFading(token);
    }

    public void FadeOutImmediately()
    {
        ChangeFadeImmediately(0);
    }
    
    public void FadeInImmediately()
    {
        ChangeFadeImmediately(1);
    }

    public void ChangeFadeImmediately(float alpha)
    {
        foreach (var accessory in _skinReferencesResolver.Accessories)
        {
            if (alpha < 1) accessory.gameObject.SetActive(false);
            else if (alpha == 1) accessory.gameObject.SetActive(true);
        }

        foreach (var meshRenderer in _skinReferencesResolver.MeshRenderers)
        {
            var material = meshRenderer.material;
            SetTransparent(material);
            
            var color = material.color;
            color.a = alpha;
            material.color = color;
            
            if (alpha == 1) SetOpaque(material);
        }
    }
    
    public void FadeOut(Action callback = null)
    {
        KillSequence();
        _fadeSequence = DOTween.Sequence();
        IsFading = true;

        foreach (var accessory in _skinReferencesResolver.Accessories)
        {
            accessory.gameObject.SetActive(false);
        }

        foreach (var meshRenderer in _skinReferencesResolver.MeshRenderers)
        {
            var material = meshRenderer.material;
            SetTransparent(material);
            
            _fadeSequence.Join(material.DOFade(0, 2));
        }
        
        _fadeSequence.OnComplete(() =>
        {
            _fadeSequence = null;
            IsFading = false;
            callback?.Invoke();
        });
    }

    public void FadeIn(Action callback = null)
    {
        KillSequence();
        _fadeSequence = DOTween.Sequence();
        IsFading = true;
        
        foreach (var meshRenderer in _skinReferencesResolver.MeshRenderers)
        {
            var material = meshRenderer.material;
            _fadeSequence.Join(material.DOFade(1, 2).OnComplete(() => SetOpaque(material)));
        }

        _fadeSequence.OnComplete(() =>
        {
            foreach (var accessory in _skinReferencesResolver.Accessories)
            {
                accessory.gameObject.SetActive(true);
            }
            
            _fadeSequence = null; 
            IsFading = false;
            callback?.Invoke();
        });
    }
    
    public async UniTask WaitFading(CancellationToken token)
    {
        await UniTask.WaitWhile(() => IsFading, cancellationToken: token);
    }

    private void KillSequence()
    {
        _fadeSequence?.Kill(true);
        _fadeSequence = null;
        IsFading = false;
    }
    
    public void Dispose()
    {
        KillSequence();
    }
    
    private void SetTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3);

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);

        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
    
    private void SetOpaque(Material mat)
    {
        mat.SetFloat("_Mode", 0);

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);

        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        mat.renderQueue = -1;
    }
}