#if UNITY_EDITOR

using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

public static class AnchorHelper
{
    private const string AnchorAsset = "Anchor";
    private const string States = "States";
    private const string Targeted = "Targeted";

    [MenuItem("GameObject/Village/Anchor", false, 10)]
    static void CreateAnchor(MenuCommand menuCommand)
    {
        var anchorSystem = UnityEngine.SceneManagement.SceneManager
            .GetActiveScene()
            .GetRootGameObjects()
            .FirstOrDefault(it => it.GetComponentInChildren<AnchorMode>(true) != null)
            .GetComponentInChildren<AnchorMode>(true);

        if (anchorSystem == null)
        {
            Debug.LogErrorFormat("Anchor Creation Faild: No Anchor System found on the scene.");
            return;
        }
        var targeted = anchorSystem.transform.Find(Targeted);

        var prefab = EditorHelper.FindAssetOfType<GameObject>(AnchorAsset);
        var root = PrefabUtility.InstantiatePrefab(prefab).GameObject();

        GameObjectUtility.SetParentAndAlign(root, menuCommand.context as GameObject);

        var states = root.transform.Find(States);
        states.GetComponent<DirectActivator>().Source = anchorSystem.GetComponent<ObservableBehaviour>();
        states.GetComponent<AndGateActivator>().SourceA = targeted.GetComponent<ObservableBehaviour>();

        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
        Selection.activeObject = root;
    }
}

#endif