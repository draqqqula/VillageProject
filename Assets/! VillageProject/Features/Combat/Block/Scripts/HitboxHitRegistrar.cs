using System;
using Zenject;

public class HitboxHitRegistrar : HitRegistrar
{
    public override event Action OnHit;
    private HitboxEvent _hitboxEvent;

    public HitboxHitRegistrar(HitboxEvent hitboxEvent)
    {
        _hitboxEvent = hitboxEvent;
        _hitboxEvent.OnHit += InvokeHitEvent;
    }

    private void InvokeHitEvent()
    {
        OnHit?.Invoke();
    }

    public override void Activate()
    {
        _hitboxEvent.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        _hitboxEvent.gameObject.SetActive(false);
    }

    public override void Dispose()
    {
        _hitboxEvent.OnHit -= InvokeHitEvent;
    }
}