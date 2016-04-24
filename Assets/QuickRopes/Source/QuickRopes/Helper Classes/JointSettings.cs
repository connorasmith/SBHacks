using UnityEngine;
using System.Collections;

namespace PicoGames.QuickRopes
{
    [System.Serializable]
    public struct JointSettings
    {
        [SerializeField, Min(0.001f)]
        public float breakForce;
        [SerializeField, Min(0.001f)]
        public float breakTorque;

        [SerializeField, Range(0, 180)]
        public float twistLimit;
        [SerializeField, Range(0, 180)]
        public float swingLimit;
        [SerializeField, Min]
        public float spring;
        [SerializeField, Min]
        public float damper;
    }
}