using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AlignerByPixels : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] public bool _isAlignDaughterImages;
    
    [ContextMenu("CheckCanvasOverlay")]
    private void CheckCanvasOverlay()
    {
        Camera targetCamera = Camera.main;
        RectTransform rect = _canvas.GetComponent<RectTransform>();
        
        Debug.Log($"Current scale factor: {_canvas.scaleFactor}");
        
        Debug.Log($"Canvas size in UNITS: {rect.rect.size}");
        Vector2 actualPixelSize = UnitsToPixels(rect.rect.size);
        Debug.Log($"Canvas size in PIXELS: {actualPixelSize}");
        
        Debug.Log($"1 unit = {_canvas.scaleFactor} screen pixels");
        Debug.Log($"Screen: {targetCamera.pixelWidth}x{targetCamera.pixelHeight}");
    }

    [ContextMenu("CheckImageOverlay")]
    private void CheckImageOverlay()
    {
        Image targetImage = GetComponent<Image>();
        RectTransform rect = targetImage.rectTransform;

        Vector2 sizeUnits = rect.rect.size * rect.lossyScale / _canvas.scaleFactor;
        Debug.Log($"Size in UNITS: {sizeUnits}");

        var sizePixels = UnitsToPixels(sizeUnits);
        Debug.Log($"Size in PIXELS: {sizePixels}");
        
        Debug.Log($"1 unit = {_canvas.scaleFactor} screen pixels");
    }

    [ContextMenu("AlignByPixels")]
    public void AlignByPixels()
    {
        if (_isAlignDaughterImages)
        {
            Image[] targetImages = GetComponentsInChildren<Image>();

            Undo.RecordObjects(targetImages.Select(image => image.rectTransform).ToArray<Object>(), "AlignByPixels");
            foreach (var targetImage in targetImages)
            {
                AlignByPixels(targetImage);
                EditorUtility.SetDirty(targetImage.rectTransform);
            }
        }
        else
        {
            Image targetImage = GetComponent<Image>();
            Undo.RecordObject(targetImage.rectTransform, $"Align {targetImage.gameObject.name} by pixels");
            AlignByPixels(targetImage);
            EditorUtility.SetDirty(targetImage.rectTransform);
        }
    }

    public void AlignByPixels(Image targetImage)
    {
        RectTransform rect = targetImage.rectTransform;
        
        Vector2 pixelPerfectPosition = GetPixelPerfectVector(rect.anchoredPosition);
        rect.anchoredPosition = pixelPerfectPosition;
        
        Debug.Log($"Aligned {targetImage.gameObject.name} to: Pos={pixelPerfectPosition}");
    }
    
    private Vector2 GetPixelPerfectVector(Vector2 vector)
    {
        var pixelPerfectVector = UnitsToPixels(vector);
        var roundedPixelsVector = new Vector2(Mathf.Round(pixelPerfectVector.x), Mathf.Round(pixelPerfectVector.y));
        return PixelsToUnits(roundedPixelsVector);
    }
    
    private Vector2 UnitsToPixels(Vector2 units)
    {
        return units * _canvas.scaleFactor;
    }

    private Vector2 PixelsToUnits(Vector2 pixels)
    {
        return pixels / _canvas.scaleFactor;
    }
}
