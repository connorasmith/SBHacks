using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PicoGames.Utilities
{
    [CustomEditor(typeof(RopeRenderer))]
    public class RopeRendererEditor : Editor
    {
        private RopeRenderer ropeRenderer;

        //private Spline spline;
        //private Keyframe[] curveKeys;

        void OnEnable()
        {
            ropeRenderer = target as RopeRenderer;
            //spline = ropeRenderer.GetComponent<Spline>();        
        }

        //void OnSceneGUI()
        //{
        //    curveKeys = ropeRenderer.RadiusCurve.keys;
        //    Transform transform = ropeRenderer.transform;

        //    for(int i = 0; i < curveKeys.Length; i++)
        //    {                
        //        Vector3 curvePosition = spline.GetPointOnCurve(1f - curveKeys[i].time);
        //        Vector3 curveTangent = spline.GetPointOnCurve(1f - curveKeys[i].time - 0.001f) - curvePosition;
        //        Vector3 curveHandle = Vector3.up * curveKeys[i].value;

        //        Handles.DrawWireArc(spline.GetPointOnCurve(1f - curveKeys[i].time), curveTangent, curveHandle, 360, curveKeys[i].value * 0.5f);
        //        curveKeys[i].value = Handles.ScaleSlider(curveKeys[i].value, curvePosition, curveTangent, Quaternion.identity, 1, 0);

        //        ropeRenderer.RadiusCurve.keys[i] = curveKeys[i];
        //    }
        //}

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Shape Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                ropeRenderer.EdgeCount = EditorGUILayout.IntField("Edge Count", ropeRenderer.EdgeCount);
                ropeRenderer.EdgeDetail = EditorGUILayout.IntField("Edge Detail", ropeRenderer.EdgeDetail);
                ropeRenderer.EdgeIndent = EditorGUILayout.FloatField("Edge Indent", ropeRenderer.EdgeIndent);

                EditorGUILayout.Space();

                ropeRenderer.Radius = EditorGUILayout.FloatField("Radius", ropeRenderer.Radius);
                ropeRenderer.RadiusCurve = EditorGUILayout.CurveField("Radius Curve", ropeRenderer.RadiusCurve, Color.green, new Rect(0, 0, 1, 5));
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Strand Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                ropeRenderer.StrandCount = EditorGUILayout.IntField("Strand Count", ropeRenderer.StrandCount);
                ropeRenderer.StrandOffset = EditorGUILayout.FloatField("Strand Offset", ropeRenderer.StrandOffset);
                ropeRenderer.StrandTwist = EditorGUILayout.FloatField("Strand Twist", ropeRenderer.StrandTwist);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Material Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                ropeRenderer.Material = (Material)EditorGUILayout.ObjectField("Material", ropeRenderer.Material, typeof(Material), false);
                ropeRenderer.UVTile = EditorGUILayout.Vector2Field("UV Tiling", ropeRenderer.UVTile);
                ropeRenderer.UVOffset = EditorGUILayout.Vector2Field("UV Offset", ropeRenderer.UVOffset);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                ropeRenderer.showEdges = EditorGUILayout.Toggle("Show Edges", ropeRenderer.showEdges);
                ropeRenderer.showNormals = EditorGUILayout.Toggle("Show Normals", ropeRenderer.showNormals);
                ropeRenderer.showBounds = EditorGUILayout.Toggle("Show Bounds", ropeRenderer.showBounds);
            }
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(ropeRenderer);
        }
    }
}