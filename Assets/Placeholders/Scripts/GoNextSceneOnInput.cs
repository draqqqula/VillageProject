using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GoNextSceneOnInput : InputListener
{
    [SerializeField] private string SceneName = "PlaseholderVillagePlaytest";

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
