#if UNITY_EDITOR
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnumAssetTable))]
public class AssetTableEditor : Editor
{
    private EnumAssetTable table;

    // Cached list of enum types marked with AssetKeyAttribute
    private List<Type> enumTypes;
    private string[] enumTypeNames;
    private int selectedEnumIndex = -1;

    private void OnEnable()
    {
        table = (EnumAssetTable)target;
        CacheEnumTypes();
        RestoreCurrentEnumSelection();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Asset Table Editor", EditorStyles.boldLabel);

        DrawEnumSelector();

        if (GUILayout.Button("Update Table"))
            UpdateTable();

        DrawCurrentValueTypeInfo();

        if (GUILayout.Button("Update Value Type"))
            UpdateValueType();

        if (GUILayout.Button("Reset Value Type"))
            ResetValueType();

        DrawEntriesInspector();

        serializedObject.ApplyModifiedProperties();
    }

    // ----------------------- ENUM SELECTION -----------------------

    private void CacheEnumTypes()
    {
        enumTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                Type[] t = new Type[0];
                try { t = a.GetTypes(); } catch { }
                return t;
            })
            .Where(t => t.IsEnum && t.GetCustomAttribute<AssetKeyAttribute>() != null)
            .ToList();

        enumTypeNames = enumTypes.Select(t => t.FullName).ToArray();
    }

    private void RestoreCurrentEnumSelection()
    {
        if (table.KeyValuePairs == null)
            return;

        Type groupType = table.KeyValuePairs.GetType();
        if (!groupType.IsGenericType)
            return;

        Type keyType = groupType.GetGenericArguments()[0];
        selectedEnumIndex = enumTypes.IndexOf(keyType);
    }

    private void DrawEnumSelector()
    {
        EditorGUILayout.LabelField("Enum (TKey)", EditorStyles.boldLabel);

        int newIndex = EditorGUILayout.Popup("Key Enum", selectedEnumIndex, enumTypeNames);
        if (newIndex != selectedEnumIndex)
        {
            selectedEnumIndex = newIndex;
            table.KeyValuePairs = null; // Reset group when enum changes
        }
    }

    // ----------------------- UPDATE TABLE -----------------------

    private void UpdateTable()
    {
        if (selectedEnumIndex < 0) return;

        Type enumType = enumTypes[selectedEnumIndex];

        // If KeyValuePairs is null, create default Group<enum, UnityEngine.Object>
        if (table.KeyValuePairs == null)
        {
            Type groupType = typeof(EnumAssetTable.Group<,>).MakeGenericType(enumType, typeof(UnityEngine.Object));
            table.KeyValuePairs = (EnumAssetTable.GroupBase)Activator.CreateInstance(groupType);
            serializedObject.Update();
        }

        // Extract generic args
        Type grpType = table.KeyValuePairs.GetType();
        Type keyType = grpType.GetGenericArguments()[0];
        Type valueType = grpType.GetGenericArguments()[1];

        // Read existing entries
        SerializedProperty entriesProp = serializedObject.FindProperty("KeyValuePairs")
            .FindPropertyRelative("Entries");

        var enumValues = Enum.GetValues(keyType).Cast<object>().ToList();
        var enumNames = Enum.GetNames(keyType);

        // Build dictionary for existing entries
        Dictionary<object, UnityEngine.Object> currentValues = new();

        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            SerializedProperty element = entriesProp.GetArrayElementAtIndex(i);
            var keyProp = element.FindPropertyRelative("Key");
            var valProp = element.FindPropertyRelative("Value");

            object keyVal = Enum.ToObject(keyType, keyProp.enumValueIndex);
            currentValues[keyVal] = valProp.objectReferenceValue;
        }

        // Sort enum values by underlying int if numeric enum
        bool numeric = IsNumericEnum(keyType);

        if (numeric)
            enumValues = enumValues.OrderBy(v => Convert.ToInt64(v)).ToList();
        else
            enumValues = enumValues.OrderBy(v => Array.IndexOf(enumNames, v.ToString())).ToList();

        // Rewrite entries
        entriesProp.ClearArray();
        for (int i = 0; i < enumValues.Count; i++)
        {
            entriesProp.InsertArrayElementAtIndex(i);
            var element = entriesProp.GetArrayElementAtIndex(i);

            SerializedProperty keyProp = element.FindPropertyRelative("Key");
            SerializedProperty valProp = element.FindPropertyRelative("Value");

            object enumVal = enumValues[i];

            keyProp.enumValueIndex = Array.IndexOf(enumNames, enumVal.ToString());

            if (currentValues.TryGetValue(enumVal, out var existingVal))
                valProp.objectReferenceValue = existingVal;
            else
                valProp.objectReferenceValue = null;
        }

        EditorUtility.SetDirty(table);
        serializedObject.ApplyModifiedProperties();
    }

    private bool IsNumericEnum(Type t)
    {
        var underlying = Enum.GetUnderlyingType(t);
        return underlying == typeof(int) || underlying == typeof(byte) ||
               underlying == typeof(short) || underlying == typeof(long);
    }

    // ----------------------- UPDATE VALUE TYPE -----------------------

    private void DrawCurrentValueTypeInfo()
    {
        if (table.KeyValuePairs == null)
        {
            EditorGUILayout.LabelField("Current TValue: <none>");
            return;
        }

        Type valType = table.KeyValuePairs.GetType().GetGenericArguments()[1];
        EditorGUILayout.LabelField("Current TValue: " + valType.FullName);
    }

    // --------------------------------------------------------
    // Rebuilds the Group<TKey, TValue> using a new TValue type.
    // Keeps all keys and values as-is.
    // --------------------------------------------------------
    private void RebuildGroup(Type newValueType)
    {
        if (table.KeyValuePairs == null)
            return;

        Type oldGroupType = table.KeyValuePairs.GetType();
        Type keyType = oldGroupType.GetGenericArguments()[0];

        // Create new Group<TKey, newValueType>
        Type newGroupType = typeof(EnumAssetTable.Group<,>).MakeGenericType(keyType, newValueType);
        var newGroup = (EnumAssetTable.GroupBase)Activator.CreateInstance(newGroupType);

        // Source entries
        SerializedProperty entriesProp = serializedObject
            .FindProperty("KeyValuePairs")
            .FindPropertyRelative("Entries");

        // Target list
        var newEntries = (IList)newGroupType
            .GetField("Entries", BindingFlags.Public | BindingFlags.Instance)
            .GetValue(newGroup);

        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            SerializedProperty entry = entriesProp.GetArrayElementAtIndex(i);

            var keyEnumIndex = entry.FindPropertyRelative("Key").enumValueIndex;
            object keyEnum = Enum.ToObject(keyType, keyEnumIndex);
            var oldValue = entry.FindPropertyRelative("Value").objectReferenceValue;

            // Create new Entry<TKey, newValueType>
            Type entryType = typeof(EnumAssetTable.Entry<,>).MakeGenericType(keyType, newValueType);
            var newEntry = Activator.CreateInstance(entryType);

            entryType.GetField("Key").SetValue(newEntry, keyEnum);
            entryType.GetField("Value").SetValue(newEntry, oldValue);

            newEntries.Add(newEntry);
        }

        table.KeyValuePairs = newGroup;
        EditorUtility.SetDirty(table);
        serializedObject.Update();
    }


    // --------------------------------------------------------
    // UpdateValueType — picks common TValue type for all assets
    // --------------------------------------------------------
    private void UpdateValueType()
    {
        if (table.KeyValuePairs == null)
            return;

        Type oldGroupType = table.KeyValuePairs.GetType();
        Type oldValueType = oldGroupType.GetGenericArguments()[1];

        SerializedProperty entriesProp = serializedObject
            .FindProperty("KeyValuePairs")
            .FindPropertyRelative("Entries");

        Type newType = GetCommonType(entriesProp);
        if (newType == null)
            newType = typeof(UnityEngine.Object);

        if (newType == oldValueType)
            return;

        RebuildGroup(newType);
    }


    // --------------------------------------------------------
    // ResetValueType — always reset TValue to UnityEngine.Object
    // --------------------------------------------------------
    private void ResetValueType()
    {
        RebuildGroup(typeof(UnityEngine.Object));
    }

    private Type GetCommonType(SerializedProperty entriesProp)
    {
        List<UnityEngine.Object> objs = new();

        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            var element = entriesProp.GetArrayElementAtIndex(i);
            var val = element.FindPropertyRelative("Value").objectReferenceValue;
            if (val != null)
                objs.Add(val);
        }

        if (objs.Count == 0)
            return typeof(UnityEngine.Object);

        Type t = objs[0].GetType();
        foreach (var obj in objs)
        {
            if (obj.GetType() != t)
                return typeof(UnityEngine.Object);
        }

        return t;
    }

    // ----------------------- ENTRY UI -----------------------

    private void DrawEntriesInspector()
    {
        if (table.KeyValuePairs == null)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Entries", EditorStyles.boldLabel);

        SerializedProperty entries = serializedObject.FindProperty("KeyValuePairs")
            .FindPropertyRelative("Entries");

        EditorGUI.indentLevel++;

        for (int i = 0; i < entries.arraySize; i++)
        {
            SerializedProperty entry = entries.GetArrayElementAtIndex(i);
            SerializedProperty keyProp = entry.FindPropertyRelative("Key");
            SerializedProperty valProp = entry.FindPropertyRelative("Value");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(keyProp, GUIContent.none);
            EditorGUILayout.PropertyField(valProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
    }
}
#endif