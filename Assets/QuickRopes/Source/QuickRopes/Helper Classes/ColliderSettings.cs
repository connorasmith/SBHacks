using UnityEngine;
using System.Collections;
using PicoGames.Utilities;

namespace PicoGames.QuickRopes
{
    [System.Serializable]
    public struct ColliderSettings
    {
        public enum Direction
        {
            X_Axis,
            Y_Axis,
            Z_Axis
        }
        [SerializeField]
        public QuickRope.ColliderType type;
        [SerializeField]
        public Direction direction;
        [SerializeField]
        public Vector3 center;
        [SerializeField]
        public Vector3 size;
        [SerializeField, Min]
        public float radius;
        [SerializeField, Min]
        public float height;
        [SerializeField]
        public PhysicMaterial physicsMaterial;
    }
}