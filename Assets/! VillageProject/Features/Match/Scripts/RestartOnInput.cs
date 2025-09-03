using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RestartOnInput : InputListener
{
    private const string SceneName = "PlaseholderVillage";

    [SerializeField, FromInputActionAsset("Jump")] public InputActionReference Restart;

    private void OnEnable()
    {
        Restart.action.performed += HandleRestart;
    }

    private void OnDisable()
    {
        Restart.action.performed -= HandleRestart;
    }

    public void HandleRestart(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneName);
    }
}
