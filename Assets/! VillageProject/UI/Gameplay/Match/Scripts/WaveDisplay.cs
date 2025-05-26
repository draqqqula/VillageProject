using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class WaveDisplay : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent _text;
    [SerializeField] private AudioSource _sound;
    private StringVariable _variable;

    public void Show(int wave)
    {
        if (_variable == null)
        {
            _variable = _text.StringReference.Values.First(it => it is StringVariable) as StringVariable;
        }
        _variable.Value = MathExtensions.ToRoman(wave);
        gameObject.SetActive(true);
        _sound.Play();
    }
}
