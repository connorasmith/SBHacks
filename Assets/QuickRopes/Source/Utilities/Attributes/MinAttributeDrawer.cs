using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(MinAttribute))]
#endif

public class MinAttributeDrawer
#if UNITY_EDITOR
    : PropertyDrawer
#endif
{
#if UNITY_EDITOR
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as MinAttribute;

        if (property.propertyType == SerializedPropertyType.Float)
            property.floatValue = EditorGUI.FloatField(position, label, attr.GetValue(property.floatValue));
        
        if (property.propertyType == SerializedPropertyType.Integer)
            property.intValue = EditorGUI.IntField(position, label, attr.GetValue(property.intValue));
    }
#endif
}