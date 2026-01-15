using R3;
using UnityEngine;

public class MoveParamUpdater
{
    public ReadOnlyReactiveProperty<float> MoveParameter => _moveParam;
    private ReactiveProperty<float> _moveParam = new ReactiveProperty<float>();
    
    public bool IsReleasing {get; private set;}
    public bool IsDecreasing {get; private set;}
    
    public void IncreaseParam(float value)
    {
        IsReleasing = false;
        IsDecreasing = false;
        
        _moveParam.Value = Mathf.Clamp(_moveParam.Value + value, 0, 1);
    }
    
    public void DecreaseParam(float value)
    {
        IsReleasing = false;
        IsDecreasing = true;
        
        _moveParam.Value = Mathf.Clamp(_moveParam.Value - value, 0, 1);
    }

    public void ReleaseParam(float releaseValue)
    {
        if (MoveParameter.CurrentValue >= 0.5f)
        {
            IncreaseParam(releaseValue);
        }
        else
        {
            DecreaseParam(releaseValue);
        }
        
        IsReleasing = true;
    }
    
    public void ReleaseParam()
    {
        _moveParam.Value = 0;
    }

    public bool IsParamOnRange(float val1, float val2, bool includeWhenReleasing)
    {
        if (includeWhenReleasing) return val1 < _moveParam.Value && _moveParam.Value < val2;
        else return val1 < _moveParam.Value && _moveParam.Value < val2 && !IsReleasing;
    }
}