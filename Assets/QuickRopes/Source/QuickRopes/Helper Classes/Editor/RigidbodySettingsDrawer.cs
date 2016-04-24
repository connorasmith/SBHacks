using UnityEngine;
using UnityEditor;

using System.Collections;

namespace PicoGames.QuickRopes
{
    [CustomPropertyDrawer(typeof(RigidbodySettings))]
    public class RigidbodySettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                if (EditorGUI.PropertyField(position, property))
                {
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("mass"));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("drag"));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("angularDrag"));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("useGravity"));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("isKinematic"));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("interpolate"));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("collisionDetection"));

                        int constraintValue = property.FindPropertyRelative("constraints").intValue;

                        EditorGUILayout.LabelField("Position Constraints", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150));
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(25);
                            SetBit("X", 2, ref constraintValue);
                            SetBit("Y", 4, ref constraintValue);
                            SetBit("Z", 8, ref constraintValue);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.LabelField("Rotation Constraints", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150));
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(25);
                            SetBit("X", 16, ref constraintValue);
                            SetBit("Y", 32, ref constraintValue);
                            SetBit("Z", 64, ref constraintValue);
                        }
                        EditorGUILayout.EndHorizontal();

                        property.FindPropertyRelative("constraints").intValue = constraintValue;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.EndProperty();
        }

        void SetBit(string _label, int _bitValue, ref int _constraintValue)
        {
            if (EditorGUILayout.ToggleLeft(_label, ((_constraintValue & _bitValue) != 0), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(70)))
                _constraintValue |= _bitValue;
            else
                _constraintValue &= ~_bitValue;
        }
    }
}