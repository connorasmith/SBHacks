using UnityEngine;
using System.Collections;

namespace PicoGames.QuickRopes
{
    [System.Serializable]
    public struct TransformSettings
    {
        [SerializeField]
        public Vector3 position;
        [SerializeField]
        public Vector3 eulerRotation;
        [SerializeField]
        public Quaternion rotation { get { return Quaternion.Euler(eulerRotation); } set { eulerRotation = value.eulerAngles; } }
        [SerializeField]
        public Vector3 scale;
    }
}