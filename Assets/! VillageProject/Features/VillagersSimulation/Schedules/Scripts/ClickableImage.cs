using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ClickableImage : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    private Image _image;
    public Image Image => _image ?? InitImage();
    public event Action<Image> OnImageTriggered;

    private Image InitImage()
    {
        _image = GetComponent<Image>();
        return _image;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnImageTriggered?.Invoke(Image);
        }
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            OnImageTriggered?.Invoke(Image);
        }
    }
}