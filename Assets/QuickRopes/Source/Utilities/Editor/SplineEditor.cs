using UnityEngine;
using UnityEditor;
using System.Collections;

using PicoGames.EditorExtensions;

namespace PicoGames.Utilities
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor
    {
        private Transform transform;
        private Spline spline;
        private float handleSize = 0.1f;
        private int selectedControl = -1;

        public static bool ShowInspector = true;
        public static bool ShowEditor = true;
        public static bool ShowEditorInPlayMode = false;

        void OnEnable()
        {
            transform = (serializedObject.targetObject as Spline).transform;
            spline = (serializedObject.targetObject as Spline);

            ShowEditor = EditorPrefs.GetBool("PicoGames_ShowSplineEditor", true);
            ShowEditorInPlayMode = EditorPrefs.GetBool("PicoGames_ShowSplineEditor_PlayMode", false);

            Tools.hidden = true;
        }

        void OnDisable()
        {
            EditorPrefs.SetBool("PicoGames_ShowSplineEditor", ShowEditor);
            EditorPrefs.SetBool("PicoGames_ShowSplineEditor_PlayMode", ShowEditorInPlayMode);
        }

        bool isLooped = false;
        bool evenDistribution = true;
        Spline.ControlPointMode mode = Spline.ControlPointMode.Mirrored;
        Vector3 controlPosition = Vector3.zero;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            if (ShowInspector)
            {
                EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                //bool showEditor = EditorGUILayout.Toggle("Show Editor", spline.ShowEditor);
                isLooped = EditorGUILayout.Toggle("Looped", spline.IsLooped);
                evenDistribution = EditorGUILayout.Toggle("Evenly Spaced", spline.EvenPointDistribution);
                //int resolution = EditorGUILayout.IntSlider("Algorithm Resolution", spline.OutputResolution / 500, 1, 10) * 500;
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Scene Editor", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                ShowEditor = EditorGUILayout.Toggle("Enabled", ShowEditor);
                ShowEditorInPlayMode = EditorGUILayout.Toggle("Show Play Mode", ShowEditorInPlayMode);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Point Editor", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if(selectedControl >= 0)
            {
                mode = (Spline.ControlPointMode)EditorGUILayout.EnumPopup("Control Mode", spline.GetMode(selectedControl - 1 / 3));
                controlPosition = EditorGUILayout.Vector3Field("Local Position", spline.GetPoint(selectedControl));

                GUI.enabled = ((0 != selectedControl) && (spline.Points.Length - 1) != selectedControl);
                if (GUILayout.Button("Delete Control"))
                {
                    Undo.RecordObject(spline, "Deleted Spline Point");

                    spline.RemoveCurve(selectedControl / 3);
                    selectedControl = -1;

                    EditorUtility.SetDirty(spline);
                }
                GUI.enabled = true;
            }
            else
            {
                if(selectedControl == -1)
                    EditorGUILayout.HelpBox("Root point is uneditable from this editor.", MessageType.Info);
                else
                    EditorGUILayout.HelpBox("Select a point to edit the spline.", MessageType.Info);
            }
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                spline.IsLooped = isLooped;
                spline.EvenPointDistribution = evenDistribution;

                if (selectedControl >= 0)
                    spline.SetPoint(selectedControl, controlPosition);

                spline.SetMode(selectedControl - 1 / 3, mode);
                SceneView.RepaintAll();
            }
        }

        void OnSceneGUI()
        {
            if (!SplineEditor.ShowEditor || (Application.isPlaying && !ShowEditorInPlayMode))
                return;
            
            #region Keypress Events
            // Get Events
            float btnSize;
            int pointsToDraw = spline.IsLooped ? spline.Points.Length - 1 : spline.Points.Length;
            //bool cntrlDwn = false;
            
            if (!Event.current.control)
            {
                btnSize = HandleUtility.GetHandleSize(transform.position) * handleSize;
                if (EditorX.CircleButton(transform.position, Color.red, btnSize))
                {
                    selectedControl = -1;
                    Tools.hidden = false;
                }

                if (Tools.hidden && selectedControl == -1)
                    Tools.hidden = false;
            }
            else
            {
                Tools.hidden = true;
            }
            
            // Draw Spline
            for (int i = 0; i < pointsToDraw; i++)
            {               
                Vector3 cpPos = transform.TransformPoint(spline.GetPoint(i));
                btnSize = HandleUtility.GetHandleSize(cpPos) * handleSize;

                bool isPivot = (i % 3) == 0;
                if (selectedControl == i)
                {
                    EditorX.DrawDot(cpPos, Color.yellow, isPivot ? btnSize : btnSize * 0.75f);

                    EditorGUI.BeginChangeCheck();

                    Vector3 newPos = transform.InverseTransformPoint(Handles.DoPositionHandle(cpPos, Quaternion.identity));

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(spline, "Moved Control Point");
                        spline.SetPoint(i, newPos);
                    }
                }
                else
                {
                    if (i == 0 && !Event.current.control)
                        continue;

                    if (EditorX.CircleButton(cpPos, isPivot ? Color.blue : Color.cyan, isPivot ? btnSize : btnSize * 0.75f))
                    {
                        selectedControl = i;
                        Tools.hidden = true;

                        Repaint();
                    }
                }
            }
                       
            for (int i = 0; i < spline.ControlCount; i++)
            {
                Vector3 p0 = transform.TransformPoint(spline.GetPoint((i * 3) + 0));
                Vector3 p1 = transform.TransformPoint(spline.GetPoint((i * 3) + 1));
                Vector3 p2 = transform.TransformPoint(spline.GetPoint((i * 3) + 2));
                Vector3 p3 = transform.TransformPoint(spline.GetPoint((i * 3) + 3));

                Handles.color = Color.green;
                Handles.DrawDottedLine(p0, p3, 5f);

                Handles.color = Color.black;
                Handles.DrawAAPolyLine(null, 2f, p0, p1);
                Handles.DrawAAPolyLine(null, 2f, p2, p3);

                Vector3 center = (p0 + p3) * 0.5f;
                float handleSz = HandleUtility.GetHandleSize(center) * handleSize;

                if (EditorX.CircleButton(center, Color.green, handleSz * 0.7f))
                {
                    Undo.RecordObject(spline, "Added Spline Point");
                    spline.AddCurve(i);
                    Repaint();
                }
            }

            DrawSpline();

            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        switch (e.keyCode)
                        {
                            case KeyCode.Backspace:
                                {
                                    // Delete Selected Control
                                    Undo.RecordObject(spline, "Deleted Spline Point");

                                    spline.RemoveCurve(selectedControl / 3);
                                    selectedControl = -1;

                                    EditorUtility.SetDirty(spline);
                                    e.Use();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }
            #endregion
        }

        private void DrawSpline()
        {
            for (int i = 0; i < spline.ControlCount; i++)
            {
                Vector3 p0 = transform.TransformPoint(spline.GetPoint((i * 3) + 0));
                Vector3 p1 = transform.TransformPoint(spline.GetPoint((i * 3) + 1));
                Vector3 p2 = transform.TransformPoint(spline.GetPoint((i * 3) + 2));
                Vector3 p3 = transform.TransformPoint(spline.GetPoint((i * 3) + 3));

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            }
        }
    }
}