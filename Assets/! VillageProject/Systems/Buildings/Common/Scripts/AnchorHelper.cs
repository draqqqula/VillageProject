#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class AnchorHelper
{
    private const string NewAnchor = "New Anchor";
    private const string Overlay = "Overlay";
    private const string Links = "Links";
    private const string LineRendererPrefabAsset = "LinkLine";

    [MenuItem("GameObject/Village/Anchor", false, 10)]
    static void CreateAnchor(MenuCommand menuCommand)
    {
        GameObject root = new GameObject(NewAnchor);

        GameObjectUtility.SetParentAndAlign(root, menuCommand.context as GameObject);
        var anchor = root.AddComponent<Anchor>();
        var linksObjects = new GameObject(Links);
        linksObjects.transform.parent = root.transform;
        linksObjects.layer = LayerMask.NameToLayer(Overlay);
        var linkDisplay = linksObjects.AddComponent<LinkDisplay>();
        linkDisplay.Prefab = EditorHelper.FindAssetOfType<GameObject>(LineRendererPrefabAsset);

        var activateOnSelected = root.AddComponent<ActivateOnSelected>();
        activateOnSelected.Target = linksObjects;
        linksObjects.SetActive(false);

        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
        Selection.activeObject = root;
    }
}

#endif