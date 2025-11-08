using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DamageAttributes))]
public class DamageAttributesEditor : Editor
{
    private List<Type> _generatedTypes;
    private string[] _typeNames;
    private int _selectedIndex = -1;

    private void OnEnable()
    {
        _generatedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.GetCustomAttribute<GenerateDamageAttributeAttribute>() != null)
            .ToList();

        _typeNames = _generatedTypes.Select(t => t.FullName).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        EditorGUILayout.Space(10);
        DrawGeneratorSection();
        EditorGUILayout.Space(10);
        DrawAttributesSection();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGeneratorSection()
    {
        EditorGUILayout.LabelField("New Attribute", EditorStyles.boldLabel);

        if (_generatedTypes.Count == 0)
        {
            EditorGUILayout.HelpBox("Attributes not found.", MessageType.Info);
            return;
        }

        _selectedIndex = EditorGUILayout.Popup("Select Attribute", _selectedIndex, _typeNames);
        if (_selectedIndex >= 0 && GUILayout.Button("Add"))
        {
            AddGeneratedAttribute(_generatedTypes[_selectedIndex]);
        }
    }

    private void AddGeneratedAttribute(Type type)
    {
        var attribute = type.GetCustomAttribute<GenerateDamageAttributeAttribute>();
        var damageAttributes = (DamageAttributes)target;

        var genericType = typeof(GenericDamageAttribute<>).MakeGenericType(type);
        var instance = Activator.CreateInstance(genericType) as DamageAttributeBase;

        genericType.GetField("ServiceLifetime").SetValue(instance, attribute.Lifetime);
        genericType.GetField("Injectable").SetValue(instance, attribute.Inject);

        var initMethod = genericType.GetMethod("EnsureProperties");
        initMethod?.Invoke(instance, null);

        Undo.RecordObject(damageAttributes, "Add Damage Attribute");
        damageAttributes.Attributes.Add(instance);
        EditorUtility.SetDirty(damageAttributes);
    }

    private void DrawAttributesSection()
    {
        var damageAttributes = (DamageAttributes)target;
        var list = damageAttributes.Attributes;

        if (list == null || list.Count == 0)
        {
            EditorGUILayout.HelpBox("No attributes added yet.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField("Attribute settings", EditorStyles.boldLabel);

        for (int i = 0; i < list.Count; i++)
        {
            var attr = list[i];
            var type = attr.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GenericDamageAttribute<>))
            {
                EditorGUILayout.BeginVertical("box");

                var t = type.GetGenericArguments()[0];
                EditorGUILayout.LabelField(type.GenericTypeArguments.First().Name + " (Generated)", EditorStyles.boldLabel);
                DrawGenericDamageAttributeInspector(attr, t);

                if (GUILayout.Button("Remove"))
                {
                    Undo.RecordObject(damageAttributes, "Remove Damage Attribute");
                    list.RemoveAt(i);
                    EditorUtility.SetDirty(damageAttributes);
                    break;
                }
                if (GUILayout.Button("Update"))
                {
                    Undo.RecordObject(damageAttributes, "Updated Damage Attribute");

                    var initMethod = type.GetMethod("EnsureProperties");
                    initMethod?.Invoke(attr, null);

                    EditorUtility.SetDirty(damageAttributes);
                    break;
                }
                if (i > 0)
                {
                    if (GUILayout.Button("Up", GUILayout.Width(30)))
                    {
                        Undo.RecordObject(damageAttributes, "Move Damage Attribute Up");
                        (damageAttributes.Attributes[i - 1], damageAttributes.Attributes[i]) =
                            (damageAttributes.Attributes[i], damageAttributes.Attributes[i - 1]);
                        EditorUtility.SetDirty(damageAttributes);
                        break;
                    }
                }
                if (i < damageAttributes.Attributes.Count - 1)
                {
                    if (GUILayout.Button("Down", GUILayout.Width(45)))
                    {
                        Undo.RecordObject(damageAttributes, "Move Damage Attribute Down");
                        (damageAttributes.Attributes[i + 1], damageAttributes.Attributes[i]) =
                            (damageAttributes.Attributes[i], damageAttributes.Attributes[i + 1]);
                        EditorUtility.SetDirty(damageAttributes);
                        break;
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }
    }

    private void DrawGenericDamageAttributeInspector(DamageAttributeBase attr, Type executableType)
    {
        var genericType = attr.GetType();
        var propertyListField = genericType.GetField("PropertyValues", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        var propertyList = propertyListField?.GetValue(attr) as List<SerializedPropertyValueBase>;

        if (propertyList == null)
        {
            return;
        }

        var fields = executableType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<FromDamageAttributePropertyAttribute>() != null)
            .ToList();

        foreach (var entry in propertyList)
        {
            entry.DrawGUI();
        }
    }
}