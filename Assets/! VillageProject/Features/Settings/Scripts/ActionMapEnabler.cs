using UnityEngine;
using UnityEngine.InputSystem;

public class ActionMapEnabler : MonoBehaviour
{
    [SerializeField] private string _inputActionMapName;
    [SerializeField] private InputActionAsset _asset;
    
    private void Awake()
    {
        _asset.FindActionMap(_inputActionMapName).Enable();
    }
}