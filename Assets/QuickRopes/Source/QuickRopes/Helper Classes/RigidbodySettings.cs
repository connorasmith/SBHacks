using UnityEngine;
using System.Collections;

namespace PicoGames.QuickRopes
{
    [System.Serializable]
    public struct RigidbodySettings
    {
        [Min(0.001f)]
        public float mass;

        [Min(0f)]
        public float drag;

        [Min(0f)]
        public float angularDrag;

        public bool useGravity;
        public bool isKinematic;
        public RigidbodyInterpolation interpolate;
        public CollisionDetectionMode collisionDetection;

        [SerializeField]
        public RigidbodyConstraints constraints;

        [Range(6, 100)]
        public int solverCount;
    }
}