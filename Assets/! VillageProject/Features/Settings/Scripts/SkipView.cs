using UnityEngine;
using UnityEngine.InputSystem;

public class SkipView : MonoBehaviour
{
    [SerializeField] private string _inputActionMapName;
    [SerializeField] private InputActionAsset _asset;

    private void OnEnable()
    {
        _asset.FindActionMap(_inputActionMapName).Disable();
    }

    private void OnDisable()
    {
        _asset.FindActionMap(_inputActionMapName).Enable();
    }
}