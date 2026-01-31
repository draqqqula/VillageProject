using R3;
using UnityEngine;

public interface ISettingsProvider
{
    public ReadOnlyReactiveProperty<T> GetSetting<T>(string name);
}
