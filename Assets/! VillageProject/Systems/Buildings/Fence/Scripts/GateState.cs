using UnityEngine;

public class GateState : MonoBehaviour
{
    private const string Opened = "Opened";
    private const string Broken = "Broken";
    private const string Damage = "Damage";

    public enum State
    {
        Closed,
        Opened,
        Broken
    }

    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _hitbox;
    [field: SerializeField] public State Current { get; private set; }
    
    public void Break()
    {
        Current = State.Broken;
        _animator.SetBool(Broken, true);
        _animator.SetBool(Opened, false);
        _animator.ResetTrigger(Damage);
    }

    public void Close()
    {
        if (Current == State.Broken)
        {
            return;
        }
        Current = State.Closed;
        _animator.SetBool(Broken, false);
        _animator.SetBool(Opened, false);
        _animator.ResetTrigger(Damage);
        _hitbox.SetActive(true);
    }

    public void Open()
    {
        if (Current == State.Broken)
        {
            return;
        }
        Current = State.Opened;
        _animator.SetBool(Broken, false);
        _animator.SetBool(Opened, true);
        _animator.ResetTrigger(Damage);
        _hitbox.SetActive(false);
    }
}
