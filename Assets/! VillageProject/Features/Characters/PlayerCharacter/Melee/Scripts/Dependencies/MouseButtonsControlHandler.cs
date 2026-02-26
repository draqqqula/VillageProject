using System;
using System.Collections;
using R3;
using UnityEngine;

public class MouseButtonsControlHandler : IDisposable
{
    //public ReadOnlyReactiveProperty<bool> IsAttackHolding => _isAttackHolding;
    public ReadOnlyReactiveProperty<bool> IsBlockHolding => _isBlockHolding;

    public ReadOnlyReactiveProperty<bool> IsAttackHolding;

    private ReactiveProperty<bool> _isAttackHolding;
    private ReactiveProperty<bool> _isBlockHolding;
    public event Action<Vector2> OnAttack;
    
    private InputWithHolding _blockInput;
    private InputWithHolding _leftAttack;
    private InputWithHolding _rightAttack;
    
    private CompositeDisposable _disposables = new CompositeDisposable();
    private CoroutineHandler _coroutineHandler;

    private Vector2 _lastDirection = Vector2.zero;
    
    public MouseButtonsControlHandler(InputWithHolding leftAttack, InputWithHolding rightAttack, InputWithHolding blockInput, CoroutineHandler coroutineHandler)
    {
        _isAttackHolding = new ReactiveProperty<bool>();
        _isBlockHolding = new ReactiveProperty<bool>();
        
        _blockInput = blockInput;
        _blockInput.IsHolding.Skip(1).Subscribe(HandleBlock).AddTo(_disposables);
        
        _leftAttack = leftAttack;
        _leftAttack.IsHolding.Skip(1).Subscribe(HandleLeftAttack).AddTo(_disposables);
        
        _rightAttack = rightAttack;
        _rightAttack.IsHolding.Skip(1).Subscribe(HandleRightAttack).AddTo(_disposables);
        
        _coroutineHandler = coroutineHandler;
        
        IsAttackHolding = _leftAttack.IsHolding.CombineLatest(_rightAttack.IsHolding, (a, b) => a || b).ToReadOnlyReactiveProperty();
    }

    private void HandleLeftAttack(bool value)
    {
        HandleAttack(value);
        if (value)
        {
            _lastDirection = GetDirection();
            OnAttack?.Invoke(_lastDirection);
        }
        else
        {
            _coroutineHandler.StartCoroutine(ReleaseWithDelay());
        }
    }
    
    private void HandleRightAttack(bool value)
    {
        HandleAttack(value);
        if (value)
        {
            _lastDirection = GetDirection();
            OnAttack?.Invoke(_lastDirection);
        }
        else
        {
            _coroutineHandler.StartCoroutine(ReleaseWithDelay());
        }
    }

    private IEnumerator ReleaseWithDelay()
    {
        if (_lastDirection == Vector2.zero)
        {
            yield return new WaitForSeconds(0.1f);
            if (_leftAttack.IsHolding.CurrentValue || _rightAttack.IsHolding.CurrentValue)
            {
                _lastDirection = GetDirection();
                OnAttack?.Invoke(_lastDirection);
            }
            else OnAttack?.Invoke(Vector2.zero);
        }
        else
        {
            if (!_leftAttack.IsHolding.CurrentValue && !_rightAttack.IsHolding.CurrentValue) OnAttack?.Invoke(_lastDirection);
        }
    }

    private void HandleBlock(bool value)
    {
        _isBlockHolding.Value = value;
    }

    private void HandleAttack(bool value)
    {
        _isAttackHolding.Value = value;
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }

    public Vector2 GetDirection()
    {
        return (_leftAttack.IsHolding.CurrentValue ? Vector2.left : Vector2.zero) + (_rightAttack.IsHolding.CurrentValue ? Vector2.right : Vector2.zero);
    }
}