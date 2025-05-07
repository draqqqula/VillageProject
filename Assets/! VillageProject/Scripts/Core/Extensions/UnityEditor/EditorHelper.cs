#if UNITY_EDITOR

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public static class EditorHelper
{
    public static T FindFirstAssetOfType<T>() where T : UnityEngine.Object
    {
        var guid = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}").FirstOrDefault();
        return LoadByGuid<T>(guid);
    }

    public static T FindAssetOfType<T>(string name) where T : UnityEngine.Object
    {
        var guid = UnityEditor.AssetDatabase.FindAssets(name).FirstOrDefault();
        return LoadByGuid<T>(guid);
    }

    public static T LoadByGuid<T>(string guid) where T : UnityEngine.Object
    {
        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
    }
}

#endif