using System;
using UnityEditor;
using UnityEngine;

public enum JsonType
{
    Null,
    Boolean,
    IntegerNumber,
    FloatNumber,
    String,
    Array,
    Object
}

[Serializable]
public class JsonValue
{
    public JsonType type;
    public bool booleanValue;
    public int intValue;
    public double floatValue;
    public string stringValue;
    public JsonValue[] arrayValue;
    [Serializable]
    public class KeyValue
    {
        public string key;
        public JsonValue value;
    }
    public KeyValue[] objectValue;

    public string ToJson()
    {
        switch (type)
        {
            case JsonType.Null:
                return "null";
            case JsonType.Boolean:
                return booleanValue ? "true" : "false";
            case JsonType.IntegerNumber:
                return intValue.ToString();
            case JsonType.FloatNumber:
                return floatValue.ToString();
            case JsonType.String:
                return "\"" + System.Text.RegularExpressions.Regex.Replace(stringValue, @"[\\\""'\b\f\n\r\t]", match => match.Value switch
                {
                    "\\" => "\\\\",
                    "\"" => "\\\"",
                    "\'" => "\\\'",
                    "\b" => "\\b",
                    "\f" => "\\f",
                    "\n" => "\\n",
                    "\r" => "\\r",
                    "\t" => "\\t",
                    _ => match.Value
                }) + "\"";
            case JsonType.Array:
                return "[" + string.Join(",", Array.ConvertAll(arrayValue, x => x.ToJson())) + "]";
            case JsonType.Object:
                return "{" + string.Join(",", Array.ConvertAll(objectValue, x => new JsonValue() { type = JsonType.String, stringValue = x.key }.ToJson() + ":" + x.value.ToJson())) + "}";
            default:
                return "null";
        }
    }
}

[CustomPropertyDrawer(typeof(JsonValue))]
public class JsonValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var type = property.FindPropertyRelative("type");
        int index;
        // type が enum 値にならないバグがある
        // ref: https://discussions.unity.com/t/enum-drop-down-menu-in-inspector-for-nested-arrays/19915
        if (type.propertyType != SerializedPropertyType.Enum)
        {
            Enum v = EditorGUI.EnumPopup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), new GUIContent(label.text + " Type"), (Enum)Enum.ToObject(typeof(JsonType), type.intValue));
            type.intValue = Convert.ToInt32(v);
            index = type.intValue;
        }
        else
        {
            EditorGUI.PropertyField(position, type, new GUIContent(label.text + " Type"), true);
            index = type.enumValueIndex;
        }
        position.y += EditorGUIUtility.singleLineHeight;

        EditorGUI.indentLevel++;

        switch ((JsonType)index)
        {
            case JsonType.Null:
                break;
            case JsonType.Boolean:
                var booleanValue = property.FindPropertyRelative("booleanValue");
                EditorGUI.PropertyField(position, booleanValue, new GUIContent("Boolean"), true);
                break;
            case JsonType.IntegerNumber:
                var intValue = property.FindPropertyRelative("intValue");
                EditorGUI.PropertyField(position, intValue, new GUIContent("Integer"), true);
                break;
            case JsonType.FloatNumber:
                var floatValue = property.FindPropertyRelative("floatValue");
                EditorGUI.PropertyField(position, floatValue, new GUIContent("Float"), true);
                break;
            case JsonType.String:
                var stringValue = property.FindPropertyRelative("stringValue");
                EditorGUI.PropertyField(position, stringValue, new GUIContent("String"), true);
                break;
            case JsonType.Array:
                var arrayValue = property.FindPropertyRelative("arrayValue");
                EditorGUI.PropertyField(position, arrayValue, new GUIContent("Array"), true);
                break;
            case JsonType.Object:
                var objectValue = property.FindPropertyRelative("objectValue");
                EditorGUI.PropertyField(position, objectValue, new GUIContent("Object"), true);
                break;
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var type = property.FindPropertyRelative("type");
        int index;
        if (type.propertyType != SerializedPropertyType.Enum)
        {
            index = type.intValue;
        }
        else
        {
            index = type.enumValueIndex;
        }

        float additionalHeight = 0;

        switch ((JsonType)index)
        {
            case JsonType.Null:
                break;
            case JsonType.Boolean:
                var booleanValue = property.FindPropertyRelative("booleanValue");
                additionalHeight += EditorGUI.GetPropertyHeight(booleanValue);
                break;
            case JsonType.IntegerNumber:
                var intValue = property.FindPropertyRelative("intValue");
                additionalHeight += EditorGUI.GetPropertyHeight(intValue);
                break;
            case JsonType.FloatNumber:
                var floatValue = property.FindPropertyRelative("floatValue");
                additionalHeight += EditorGUI.GetPropertyHeight(floatValue);
                break;
            case JsonType.String:
                var stringValue = property.FindPropertyRelative("stringValue");
                additionalHeight += EditorGUI.GetPropertyHeight(stringValue);
                break;
            case JsonType.Array:
                var arrayValue = property.FindPropertyRelative("arrayValue");
                additionalHeight += EditorGUI.GetPropertyHeight(arrayValue);
                break;
            case JsonType.Object:
                var objectValue = property.FindPropertyRelative("objectValue");
                additionalHeight += EditorGUI.GetPropertyHeight(objectValue);
                break;
        }
        return EditorGUIUtility.singleLineHeight + additionalHeight;
    }

}