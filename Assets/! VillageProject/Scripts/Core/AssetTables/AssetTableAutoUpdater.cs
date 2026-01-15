#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public class AssetTableAutoUpdater : AssetPostprocessor
{
    // Triggered after any asset changes
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        bool tableChanged =
            importedAssets.Any(a => a.EndsWith(".asset") && IsAssetTable(a));

        if (!tableChanged)
            return;

        UpdateProjectContextInstaller();
    }

    private static bool IsAssetTable(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
        return asset is EnumAssetTable;
    }

    private static void UpdateProjectContextInstaller()
    {
        var installer = FindProjectContextInstaller();
        if (installer == null)
            return;

        installer.GrabTables();

        EditorUtility.SetDirty(installer);
        AssetDatabase.SaveAssets();

        Debug.Log("[AssetTableInstaller] Auto-updated tables due to asset change.");
    }

    private static AssetTableInstaller FindProjectContextInstaller()
    {
        // Search for "ProjectContext.prefab" anywhere
        string[] guids = AssetDatabase.FindAssets("t:Prefab ProjectContext");

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                continue;

            var installer = prefab.GetComponent<AssetTableInstaller>();
            if (installer != null)
                return installer;
        }

        // fallback: search ANY prefab with AssetTableInstaller
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in allPrefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                continue;

            var installer = prefab.GetComponent<AssetTableInstaller>();
            if (installer != null)
                return installer;
        }

        Debug.LogWarning("AssetTableInstaller not found in any prefab.");
        return null;
    }

    class AssetTableAutoUpdaterModificationProcessor : AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions _)
        {
            bool tableChanged = assetPath.EndsWith(".asset") && IsAssetTable(assetPath);

            if (tableChanged)
            {
                EditorApplication.delayCall += () =>
                {
                    UpdateProjectContextInstaller();
                };
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }

    [MenuItem("Assets/Grab AssetTables")]
    private static void GrabAssetTablesMenu()
    {
        // Найти ProjectContext Installer
        AssetTableInstaller installer = FindProjectContextInstaller();
        if (installer != null)
        {
            installer.GrabTables();
        }
        else
        {
            Debug.LogWarning("[AssetTableInstaller] ProjectContext Installer not found.");
        }
    }
}
#endif