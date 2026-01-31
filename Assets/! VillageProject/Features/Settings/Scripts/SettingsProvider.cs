using R3;
using System;
using UnityEngine;

public class SettingsProvider
{
    public ReadOnlyReactiveProperty<T> GetSetting<T>(string name)
    {
        throw new NotImplementedException();
    }

    public void BindSetting<T>(ReactiveProperty<T> setting)
    {
        JsonUtility.ToJson(setting.CurrentValue);
    }
}
