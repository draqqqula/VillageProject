using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingMenuNavigation : InputListener
{
    [SerializeField, FromInputActionAsset("Navigate")] public InputActionReference Navigate;

    private void OnEnable()
    {
        Navigate.action.performed += HandleNaviagation;
    }

    private void OnDisable()
    {
        Navigate.action.performed -= HandleNaviagation;
    }

    private void HandleNaviagation(InputAction.CallbackContext context)
    {
        Transform current = null;
        int currentIndex = 0;
        var value = context.ReadValue<Vector2>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (EventSystem.current.currentSelectedGameObject == child)
            {
                current = child;
                currentIndex = i;
            }
        }
        if (current == null)
        {
            return;
        }

        if (value == Vector2.down && currentIndex < transform.childCount - 1)
        {
            EventSystem.current.SetSelectedGameObject(transform.GetChild(currentIndex - 1).gameObject);
        }
        else if (value == Vector2.up && currentIndex > 1)
        {
            EventSystem.current.SetSelectedGameObject(transform.GetChild(currentIndex + 1).gameObject);
        }
    }
}