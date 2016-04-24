using UnityEngine;
using UnityEditor;

using System.Collections;

namespace PicoGames.QuickRopes
{
    [CustomPropertyDrawer(typeof(ColliderSettings))]
    public class ColliderSettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                if (EditorGUI.PropertyField(position, property))
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("type"));

                    switch (property.FindPropertyRelative("type").enumValueIndex)
                    {
                        case 0: // None
                            break;
                        case 1: // Sphere
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("center"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("radius"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("physicsMaterial"));
                            break;
                        case 2: // Box
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("center"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("size"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("physicsMaterial"));
                            break;
                        case 3: // Capsule
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("center"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("direction"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("radius"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("height"));
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("physicsMaterial"));
                            break;
                    }
                }
            }
            EditorGUI.EndProperty();
        }
    }
}