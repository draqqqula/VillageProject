using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class AlignerByPixels : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _scaledParent;
    
    [SerializeField] public bool _isAlignDaughterImages;
    
    [ContextMenu("CheckCanvasOverlay")]
    private void CheckCanvasOverlay()
    {
        Camera targetCamera = Camera.main;
        RectTransform rect = _canvas.GetComponent<RectTransform>();
        
        Debug.Log($"Current scale factor: {_canvas.scaleFactor}");
        
        Debug.Log($"Canvas size in UNITS: {rect.rect.size}");
        Vector2 actualPixelSize = UnitsToPixels(rect.rect.size, rect.lossyScale.x);
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

        var sizePixels = UnitsToPixels(sizeUnits, rect.lossyScale.x);
        Debug.Log($"Size in PIXELS: {sizePixels}");
        
        Debug.Log($"1 unit = {_canvas.scaleFactor} screen pixels");
    }

    [ContextMenu("AlignByPixels")]
    public void AlignByPixels()
    {
        if (_isAlignDaughterImages)
        {
            Image[] targetImages = GetComponentsInChildren<Image>(includeInactive: true);

            #if UNITY_EDITOR
            Undo.RecordObjects(targetImages.Select(image => image.rectTransform).ToArray<Object>(), "AlignByPixels");
            #endif
            foreach (var targetImage in targetImages)
            {
                AlignByPixels(targetImage);
                # if UNITY_EDITOR
                EditorUtility.SetDirty(targetImage.rectTransform);
                #endif
            }
        }
        else
        {
            Image targetImage = GetComponent<Image>();
#if UNITY_EDITOR
            Undo.RecordObject(targetImage.rectTransform, $"Align {targetImage.gameObject.name} by pixels");
#endif
            AlignByPixels(targetImage);
            #if UNITY_EDITOR
            EditorUtility.SetDirty(targetImage.rectTransform);
            #endif
        }
    }

    public void AlignByPixels(Image targetImage)
    {
        RectTransform rect = targetImage.rectTransform;
        
        if (_scaledParent.lossyScale.x != rect.lossyScale.x) Debug.LogError("Can't align images! Parent and rect has different lossy scale!");
        
        var pivot = rect.pivot;
        ChangePivot(rect, Vector2.zero);
        
        Vector2 pixelPerfectPosition = GetPixelPerfectVector(_scaledParent.InverseTransformPoint(rect.position), _scaledParent.lossyScale.x);
        rect.position = _scaledParent.TransformPoint(pixelPerfectPosition);
        ChangePivot(rect, pivot);
        
        Debug.Log($"Aligned {targetImage.gameObject.name} to: Pos={rect.anchoredPosition}");
    }

    private void ChangePivot(RectTransform rect, Vector2 newPivot)
    {
        Vector2 deltaPivot = rect.pivot - newPivot;
        Vector2 deltaPosition = new Vector2(deltaPivot.x * rect.rect.width, deltaPivot.y * rect.rect.height);
    
        rect.pivot = newPivot;
        rect.anchoredPosition -= deltaPosition;
    }
    
    private Vector2 GetPixelPerfectVector(Vector2 vector, float totalScaleFactor)
    {
        var pixelPerfectVector = UnitsToPixels(vector, totalScaleFactor);
        var roundedPixelsVector = new Vector2(Mathf.Ceil(pixelPerfectVector.x), Mathf.Ceil(pixelPerfectVector.y));
        roundedPixelsVector = RoundByPixelSize(roundedPixelsVector, totalScaleFactor);
        
        return PixelsToUnits(roundedPixelsVector, totalScaleFactor);
    }

    private Vector2 RoundByPixelSize(Vector2 vector, float totalScaleFactor)
    {
        var rel = vector.x % totalScaleFactor;
        var inversedRel = totalScaleFactor - rel;
        
        if (rel <= inversedRel) vector -= Vector2.right * rel;
        else vector += Vector2.right * inversedRel;
        
        rel = vector.y % totalScaleFactor;
        inversedRel = totalScaleFactor - rel;
        
        if (rel <= inversedRel) vector -= Vector2.up * rel;
        else vector += Vector2.up * inversedRel;
        
        return vector;
    }
    
    private Vector2 UnitsToPixels(Vector2 units, float totalScaleFactor)
    {
        return units * totalScaleFactor;
    }

    private Vector2 PixelsToUnits(Vector2 pixels, float totalScaleFactor)
    {
        return pixels / totalScaleFactor;
    }

    private void OnValidate()
    {
        if (_canvas == null) _canvas = transform.GetComponentInParent<Canvas>();
        if (_scaledParent == null)
        {
            var curTransform = transform;
            while (curTransform.parent != _canvas.transform)
            {
                curTransform = curTransform.parent;
            }
            _scaledParent = curTransform;
        }
    }
}
