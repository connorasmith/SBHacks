using UnityEngine;
using UnityEditor;
using System.Collections;

using PicoGames.Utilities;
using PicoGames.EditorExtensions;

namespace PicoGames.QuickRopes
{
    [CustomEditor(typeof(QuickRope))]
    public class QuickRopeEditor : Editor 
    {        
        QuickRope rope;
        private static int editMode = 0;

        private float handleSize = 0.1f;
        private int selectedLink = 0;
        private int selectedLinks; 

        private System.Action[] sceneGUIDelegates;
        private System.Action[] inspectorGUIDelegats;

        private bool checkIfOld = true;

        void OnEnable()
        {
            if(checkIfOld && ValidateConvertTo315())
                ConvertTo315();
            
            rope = serializedObject.targetObject as QuickRope;

            sceneGUIDelegates = new System.Action[] { BaseSceneGUI, JointsSceneGUI, BaseSceneGUI };
            inspectorGUIDelegats = new System.Action[] { BaseInspector, LinksInspector, SplineInspector };

            if (rope.Links.Length == 0)
                UpdateRope();

            SplineEditor.ShowInspector = false;
            editMode = EditorPrefs.GetInt("PicoGames_QuickRopes_EditMode", 0);
        }

        void OnDisable()
        {
            Tools.hidden = false;
            SplineEditor.ShowEditor = true;

            EditorPrefs.SetInt("PicoGames_QuickRopes_EditMode", editMode);
        }
                
        #region Scene GUI's
        void OnSceneGUI()
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                UpdateRope();
            }
            else
            {
                sceneGUIDelegates[editMode].Invoke();

                if (rope.Spline.hasChanged)
                {
                    rope.Spline.hasChanged = false;
                    UpdateRope();
                }
            }
        }

        void BaseSceneGUI()
        {
            SplineEditor.ShowEditor = true;
        }

        void JointsSceneGUI()
        {
            SplineEditor.ShowEditor = false;

            for(int i = 0; i < rope.Links.Length; i++)
            {
                if (!rope.Links[i].IsActive)
                    continue;

                Vector3 pos = rope.Links[i].transform.position;
                float handleSz = HandleUtility.GetHandleSize(pos) * handleSize;

                if (selectedLink == i)
                {
                    EditorX.CircleButton(pos, Color.yellow, handleSz * 0.7f);                    
                }
                else
                {
                    bool isCustom = (rope.Links[i].overrideColliderSettings ||
                        rope.Links[i].overrideJointSettings ||
                        rope.Links[i].overrideOffsetSettings ||
                        rope.Links[i].overridePrefab ||
                        rope.Links[i].overrideRigidbodySettings);

                    if (EditorX.CircleButton(pos, isCustom ? new Color(0.7f, 0.3f, 0.3f) : Color.white, handleSz * (isCustom ? 0.7f : 0.5f)))
                    {
                        selectedLink = i;
                        this.Repaint();
                    }
                }
            }
        }
        #endregion

        #region Inspector GUI's
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            editMode = GUILayout.Toolbar(editMode, new string[] { "Settings", "Links", "Spline" }, EditorStyles.miniButton);
            
            EditorGUILayout.Space();
            inspectorGUIDelegats[editMode].Invoke();

            if (GUI.changed)
                SceneView.RepaintAll();
        }

        void BaseInspector()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Base Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLinkCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("linkSpacing"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("canResize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("usePhysics"));
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Render Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabPlacementMode"));                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultPrefab"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("linkScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("alternateRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultPrefabOffsets"), true);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            GUI.enabled = rope.usePhysics;
            {
                EditorGUILayout.LabelField("Physics Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {                   
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultRigidbodySettings"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultJointSettings"), true);
                    
                    GUI.enabled = true;

                    if (EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultColliderSettings")))
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultColliderSettings"), true);

                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
            GUI.enabled = true;
            
            if (serializedObject.ApplyModifiedProperties())
                UpdateRope();
        }

        void LinksInspector()
        {
            Tools.hidden = true;
            serializedObject.Update();

            selectedLink = Mathf.Clamp(selectedLink, 0, rope.ActiveLinkCount);
            if (selectedLink >= 0)
            {
                SerializedProperty link = serializedObject.FindProperty("links").GetArrayElementAtIndex(selectedLink);
                EditorGUILayout.LabelField("Editing Link: " + rope.Links[selectedLink].gameObject.name);

                EditorGUILayout.PropertyField(link.FindPropertyRelative("overridePrefab"), new GUIContent("Override Prefab", ""));
                if (link.FindPropertyRelative("overridePrefab").boolValue)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(link.FindPropertyRelative("prefab"));

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(link.FindPropertyRelative("overrideOffsetSettings"), new GUIContent("Override Offsets", ""));
                if(link.FindPropertyRelative("overrideOffsetSettings").boolValue)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(link.FindPropertyRelative("offsetSettings"), true);

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(link.FindPropertyRelative("overrideRigidbodySettings"), new GUIContent("Override Rigidbody", ""));
                if(link.FindPropertyRelative("overrideRigidbodySettings").boolValue)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(link.FindPropertyRelative("rigidbodySettings"), true);
                    
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(link.FindPropertyRelative("overrideJointSettings"), new GUIContent("Override Joint", ""));
                if (link.FindPropertyRelative("overrideJointSettings").boolValue)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(link.FindPropertyRelative("jointSettings"), true);

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(link.FindPropertyRelative("overrideColliderSettings"), new GUIContent("Override Collider", ""));
                if (link.FindPropertyRelative("overrideColliderSettings").boolValue)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(link.FindPropertyRelative("colliderSettings"), true);

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.LabelField("Select a link to edit its settings.");
            }

            if (serializedObject.ApplyModifiedProperties())
                UpdateRope();
        }

        void SplineInspector()
        {
            EditorGUI.BeginChangeCheck();

            rope.Spline.IsLooped = EditorGUILayout.Toggle("Is Looped", rope.Spline.IsLooped);
            rope.Spline.EvenPointDistribution = EditorGUILayout.Toggle("Evenly Distribution", rope.Spline.EvenPointDistribution);
            rope.Spline.outputResolution = EditorGUILayout.IntField("Output Resolution", rope.Spline.outputResolution);

            int estRes = (int)(rope.Spline.CurveLength / rope.linkSpacing) * 50;
            GUI.contentColor = (estRes > rope.Spline.outputResolution) ? Color.yellow : Color.green;
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Est. Min. Output Resolution: " + (int)(rope.Spline.CurveLength / rope.linkSpacing) * 50, EditorStyles.miniLabel);
            EditorGUI.indentLevel--;
            GUI.contentColor = Color.white;

            EditorGUILayout.Space();
            SplineEditor.ShowEditorInPlayMode = EditorGUILayout.Toggle("Edit In Play Mode", SplineEditor.ShowEditorInPlayMode);

            if (EditorGUI.EndChangeCheck())
                UpdateRope();
        }
        
        #endregion

        void UpdateRope()
        {
            if (!rope.gameObject.activeInHierarchy)
                return;

            rope.Generate();

            EditorUtility.SetDirty(rope);
            this.Repaint();
        }


        #region RUN TO UPDATE JOINT CONVERSION
        static void ConvertTo315()
        {
            GameObject go = Selection.activeGameObject;
            QuickRope rope = go.GetComponent<QuickRope>();

            for (int i = 0; i < rope.Links.Length; i++)
            {
                ConfigurableJoint oldJnt = rope.Links[i].gameObject.GetComponent<ConfigurableJoint>();
                if (oldJnt != null)
                    DestroyImmediate(oldJnt);
            }

            Debug.Log("Update Rope");
        }
        static bool ValidateConvertTo315()
        {
            GameObject go = Selection.activeGameObject;

            if (go == null)
                return false;

            QuickRope rope = go.GetComponent<QuickRope>();
            if (rope == null)
                return false;

            bool isNewest = true;
            for (int i = 0; i < rope.Links.Length; i++)
            {
                if (rope.Links[i].gameObject.GetComponent<ConfigurableJoint>() != null)
                {
                    isNewest = false;
                    break;
                }
            }

            return !isNewest;
        }
        #endregion
    }
}