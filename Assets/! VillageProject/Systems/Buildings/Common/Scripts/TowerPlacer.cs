using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : InputListener
{
    [SerializeField] private int Remaining = 3;
    [SerializeField] private AnchorView AnchorView;
    [SerializeField] private GameObject Tower;
    [SerializeField, FromInputActionAsset("Jump")] public InputActionReference Submit;

    private void OnEnable()
    {
        Submit.action.started += HandleSumbit;
    }

    private void OnDisable()
    {
        Submit.action.started -= HandleSumbit;
    }

    private void HandleSumbit(InputAction.CallbackContext context)
    {
        if (Remaining > 0)
        {
            Instantiate(Tower, AnchorView.ActiveAnchor.transform);
            Remaining -= 1;
        }
    }
}
