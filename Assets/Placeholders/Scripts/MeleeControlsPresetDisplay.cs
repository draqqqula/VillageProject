using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class MeleeControlsPresetDisplay : MonoBehaviour
{
    [SerializeField] private List<string> _names;
    [SerializeField] private TMP_Text _text;
    [Inject] private MeleeControlsPresetManager _manager;

    void Awake()
    {
        _manager.ChosenPreset.Subscribe(HandleUpdated).AddTo(this);
    }

    private void HandleUpdated(int presetIndex)
    {
        var name = _names[presetIndex];
        _text.text = name;
    }
}