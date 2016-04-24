using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PicoGames.EditorExtensions
{
    public static class EditorX
    {
        private static float buttonBorderSize = 1.3f;
        private static Camera sceneCamera;

        public static bool CircleButton(Vector3 _position, Color _color, float _size)
        {
            sceneCamera = SceneView.currentDrawingSceneView.camera;

            DrawDot(_position, _color, _size);
            return Handles.Button(_position, sceneCamera.transform.rotation, _size, _size * buttonBorderSize, Handles.CircleCap);
        }

        public static void DrawDot(Vector3 _position, Color _color, float _size)
        {
            sceneCamera = SceneView.currentDrawingSceneView.camera;

            Handles.color = Color.black;
            Handles.DrawSolidDisc(_position, -sceneCamera.transform.forward, _size * buttonBorderSize);

            Handles.color = _color;
            Handles.DrawSolidDisc(_position, -sceneCamera.transform.forward, _size);
        }
    }
}