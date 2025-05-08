using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchInputAction : MonoBehaviour
{
    [SerializeField] public InputActionReference Switch;
    [SerializeField] private GameObject Target;

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
        if (Target.activeSelf)
        {
            Target.SetActive(false);
        }
        else
        {
            Target.SetActive(true);
        }
    }
}
