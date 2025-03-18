using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
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
        bool IsInputActionReferenceField(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType.IsEquivalentTo(typeof(InputActionReference)) && fieldInfo.HasAttribute<SerializeField>();
        }

        string GetActionName(FieldInfo fieldInfo)
        {
            var attribute = fieldInfo.GetAttribute<FromInputActionAssetAttribute>();
            if (attribute is not null)
            {
                return attribute.ActionName;
            }
            return fieldInfo.Name;
        }

        var guid = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(InputActionAsset)}").FirstOrDefault();
        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        _inputActionsAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
        var fields = GetType().GetRuntimeFields().Where(IsInputActionReferenceField);

        foreach (var field in fields)
        {
            var name = GetActionName(field);
            var found = _inputActionsAsset.FindAction(name);
            if (found != null)
            {
                var reference = InputActionReference.Create(found);
                field.SetValue(this, reference);
            }
        }
    }
#endif
}
