using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{

    public static IEnumerable<GameObject> EnumerateImmediateChildren(this Transform transform)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            yield return transform.GetChild(i).gameObject;
        }
    }

    public static void SetLayerAllChildren(this GameObject go, string name)
    {
        foreach (var child in go.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(child.gameObject);
#endif
        }
    }
}