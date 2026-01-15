using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetTableInstaller : MonoInstaller
{
    [SerializeField] private List<EnumAssetTable> Tables = new List<EnumAssetTable>();

    public override void InstallBindings()
    {
        foreach (var item in Tables)
        {
            item.KeyValuePairs.Register(Container);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Grab Tables")]
    public void GrabTables()
    {
        // Find all assets of type AssetTable in the project
        string[] guids = AssetDatabase.FindAssets("t:EnumAssetTable");

        Tables.Clear();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnumAssetTable table = AssetDatabase.LoadAssetAtPath<EnumAssetTable>(path);

            if (table != null)
                Tables.Add(table);
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"[AssetTableInstaller] Grabbed {Tables.Count} AssetTable assets.");
    }
#endif
}