using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InputListener : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActionsAsset;

#if UNITY_EDITOR
    [ContextMenu("Assign Input")]
    private void AssignInput()
    {
        bool IsInputActionReferenceField(FieldInfo fieldInfo) =>
    fieldInfo.FieldType.IsEquivalentTo(typeof(InputActionReference)) &&
    fieldInfo.HasAttribute<SerializeField>();

        string GetActionName(FieldInfo fieldInfo)
        {
            var attribute = fieldInfo.GetAttribute<FromInputActionAssetAttribute>();
            return attribute != null ? attribute.ActionName : fieldInfo.Name;
        }

        var guid = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(InputActionAsset)}").FirstOrDefault();
        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        _inputActionsAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);

        var so = new UnityEditor.SerializedObject(this);
        var fields = GetType().GetRuntimeFields().Where(IsInputActionReferenceField);

        foreach (var field in fields)
        {
            var name = GetActionName(field);
            var found = _inputActionsAsset.FindAction(name);
            if (found == null) continue;

            var reference = InputActionReference.Create(found);

            // найти свойство по имени поля
            var sp = so.FindProperty(field.Name);
            if (sp != null)
            {
                sp.objectReferenceValue = reference;
            }
        }

        so.ApplyModifiedProperties();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }
#endif
}
