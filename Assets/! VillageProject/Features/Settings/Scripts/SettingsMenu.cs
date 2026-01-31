using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private string _inputActionMapName;
    [SerializeField] private InputActionAsset _asset;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        _asset.FindActionMap(_inputActionMapName).Disable();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        _asset.FindActionMap(_inputActionMapName).Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}