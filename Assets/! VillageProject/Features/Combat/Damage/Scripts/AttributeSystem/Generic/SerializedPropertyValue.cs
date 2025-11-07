using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class SerializedPropertyValueBase
{
    public string Name;

    public abstract object Value { get; }

    public abstract void DrawGUI();
}

[Serializable]
public class SerializedPropertyStringValue : SerializedPropertyValueBase
{
    [SerializeField] private string _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(Name), _value);
    }
}

[Serializable]
public class SerializedPropertyIntValue : SerializedPropertyValueBase
{
    [SerializeField] private int _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.IntField(ObjectNames.NicifyVariableName(Name), _value);
    }
}

[Serializable]
public class SerializedPropertyFloatValue : SerializedPropertyValueBase
{
    [SerializeField] private float _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.FloatField(ObjectNames.NicifyVariableName(Name), _value);
    }
}

[Serializable]
public class SerializedPropertyBoolValue : SerializedPropertyValueBase
{
    [SerializeField] private bool _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(Name), _value);
    }
}

[Serializable]
public class SerializedPropertyVector2Value : SerializedPropertyValueBase
{
    [SerializeField] private Vector2 _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.Vector2Field(ObjectNames.NicifyVariableName(Name), _value);
    }
}

[Serializable]
public class SerializedPropertyVector3Value : SerializedPropertyValueBase
{
    [SerializeField] private Vector3 _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.Vector3Field(ObjectNames.NicifyVariableName(Name), _value);
    }
}

[Serializable]
public class SerializedPropertyColorValue : SerializedPropertyValueBase
{
    [SerializeField] private Color _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.ColorField(ObjectNames.NicifyVariableName(Name), _value);
    }
}

[Serializable]
public class SerializedPropertyGameObjectValue : SerializedPropertyValueBase
{
    [SerializeField] private GameObject _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = (GameObject)EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(Name), _value, typeof(GameObject), false);
    }
}

[Serializable]
public class SerializedPropertySpriteValue : SerializedPropertyValueBase
{
    [SerializeField] private Sprite _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = (Sprite)EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(Name), _value, typeof(Sprite), false);
    }
}

[Serializable]
public class SerializedPropertyMaterialValue : SerializedPropertyValueBase
{
    [SerializeField] private Material _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = (Material)EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(Name), _value, typeof(Material), false);
    }
}

[Serializable]
public class SerializedPropertyObjectReferenceValue : SerializedPropertyValueBase
{
    [SerializeField] private UnityEngine.Object _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(Name), _value, typeof(UnityEngine.Object), false);
    }
}

[Serializable]
public class SerializedPropertyEnumValue<T> : SerializedPropertyValueBase where T : Enum
{
    [SerializeField] private T _value;
    public override object Value => _value;

    public override void DrawGUI()
    {
        _value = (T)EditorGUILayout.EnumPopup(ObjectNames.NicifyVariableName(Name), _value);
    }
}

public class AttributeEditorExtensions
{
    public static SerializedPropertyValueBase CreateSerializedValue(string fieldName, Type fieldType)
    {
        SerializedPropertyValueBase instance;

        if (fieldType == typeof(int))
            instance = new SerializedPropertyIntValue();
        else if (fieldType == typeof(float))
            instance = new SerializedPropertyFloatValue();
        else if (fieldType == typeof(bool))
            instance = new SerializedPropertyBoolValue();
        else if (fieldType == typeof(string))
            instance = new SerializedPropertyStringValue();
        else if (fieldType == typeof(Vector2))
            instance = new SerializedPropertyVector2Value();
        else if (fieldType == typeof(Vector3))
            instance = new SerializedPropertyVector3Value();
        else if (fieldType == typeof(Color))
            instance = new SerializedPropertyColorValue();
        else if (typeof(GameObject).IsAssignableFrom(fieldType))
            instance = new SerializedPropertyGameObjectValue();
        else if (typeof(Sprite).IsAssignableFrom(fieldType))
            instance = new SerializedPropertySpriteValue();
        else if (typeof(Material).IsAssignableFrom(fieldType))
            instance = new SerializedPropertyMaterialValue();
        else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            instance = new SerializedPropertyObjectReferenceValue();
        else if (fieldType.IsEnum)
        {
            var genericType = typeof(SerializedPropertyEnumValue<>).MakeGenericType(fieldType);
            instance = (SerializedPropertyValueBase)Activator.CreateInstance(genericType);
        }
        else
            throw new NotSupportedException($"{fieldType} is not supported");

        instance.Name = fieldName;
        return instance;
    }
}