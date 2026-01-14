using UnityEngine;

public class MaterialPropertyDebugger : MonoBehaviour
{
    [SerializeField] private Material material;

    [ContextMenu("Print")]
    private void Print()
    {
        foreach (var name in material.GetTexturePropertyNames())
            Debug.Log("Texture: " + name);

        foreach (var name in material.GetPropertyNames(MaterialPropertyType.Float))
            Debug.Log("Float: " + name);

        foreach (var name in material.GetPropertyNames(MaterialPropertyType.Vector))
            Debug.Log("Color: " + name);
    }
}