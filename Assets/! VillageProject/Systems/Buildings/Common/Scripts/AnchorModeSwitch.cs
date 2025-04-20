using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorModeSwitch : InputListener
{
    [SerializeField, FromInputActionAsset("Interact")] public InputActionReference Switch;
    [SerializeField] private GameObject AnchorView;

    private void OnEnable()
    {
        Switch.action.started += HandleSwitch;
    }

    private void OnDisable()
    {
        Switch.action.started -= HandleSwitch;
    }

    private void HandleSwitch(InputAction.CallbackContext context)
    {
        if (AnchorView.activeSelf)
        {
            AnchorView.SetActive(false);
        }
        else
        {
            AnchorView.SetActive(true);
        }
    }
}
