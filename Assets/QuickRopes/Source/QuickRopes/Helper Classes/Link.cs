using UnityEngine;
using System.Collections;

using PicoGames.Utilities;

namespace PicoGames.QuickRopes
{
    [System.Serializable]
    public class Link
    {
        [SerializeField]
        public bool overridePrefab = false;
        [SerializeField]
        public bool overrideOffsetSettings = false;
        [SerializeField]
        public bool overrideRigidbodySettings = false;
        [SerializeField]
        public bool overrideJointSettings = false;
        [SerializeField]
        public bool overrideColliderSettings = false;

        [SerializeField]
        public GameObject prefab = null;
        [SerializeField]
        public TransformSettings offsetSettings;
        [SerializeField]
        public RigidbodySettings rigidbodySettings;
        [SerializeField]
        public JointSettings jointSettings;
        [SerializeField]
        public ColliderSettings colliderSettings;

        [SerializeField]
        public bool alternateJoints = true;
        
        [SerializeField]
        public GameObject gameObject;        
        [SerializeField]
        public Transform transform { get { return gameObject.transform; } }
        [SerializeField]
        public Collider collider;

        [SerializeField]
        private GameObject attachedPrefab = null;
        [SerializeField]
        private Rigidbody rigidbody;
        [SerializeField]
        private CharacterJoint joint;
        [SerializeField]
        private int linkIndex = -1;
        [SerializeField]
        private bool prevIsKinematic = false;

        public GameObject Prefab { get{ return prefab; } set { prefab = value; } }
        public Transform Parent { get { return transform.parent; } set { transform.parent = value; } }
        public Rigidbody ConnectedBody
        {
            get { if (joint == null) return null; return joint.connectedBody; }
            set { if (joint == null) return; joint.connectedBody = value; }
        }
        public bool IsActive
        {
            get { if (gameObject == null) return false; return gameObject.activeSelf; }
            set 
            {
                if (gameObject == null)
                    return;

                gameObject.hideFlags = (value) ? HideFlags.None : HideFlags.HideInHierarchy; gameObject.SetActive(value);
                IsPrefabActive = true;
            }
        }
        public bool IsPrefabActive
        {
            get
            {
                return (attachedPrefab != null && attachedPrefab.activeSelf);
            }

            set
            {
                if (attachedPrefab == null)
                    return;

                attachedPrefab.SetActive(value);
            }
        }

        public Rigidbody Rigidbody { get { return rigidbody; } }
        public CharacterJoint Joint { get { return joint; } }
        
        public Link(GameObject _gameObject, int _index)
        {
            this.gameObject = _gameObject;
            this.linkIndex = _index;

            //ApplyRigidbodySettings();
            //ApplyJointSettings(0);

            //TogglePhysicsEnabled(true);

            IsActive = false;
        }
        
        public bool TogglePhysicsEnabled(bool _enabled)
        {
            if (_enabled)
            {
                Rigidbody.isKinematic = prevIsKinematic;
            }
            else
            {
                prevIsKinematic = Rigidbody.isKinematic;
                Rigidbody.isKinematic = true;
            }

            return _enabled;
        }
        public void ApplyPrefabSettings()
        {
            if (transform.childCount > 0)
            {
                if (!Application.isPlaying)
                {
                    while (transform.childCount > 0)
                        GameObject.DestroyImmediate(transform.GetChild(0).gameObject, false);
                }
                else
                {
                    for (int i = 0; i < transform.childCount; i++)
                        GameObject.Destroy(transform.GetChild(i).gameObject);
                }
            }

            if (prefab != null)
            {
                if (prefab.activeInHierarchy)
                {
                    attachedPrefab = prefab;
                }
                else
                {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
                    attachedPrefab = (GameObject)GameObject.Instantiate(prefab);
#else
                    attachedPrefab = GameObject.Instantiate<GameObject>(prefab);
#endif

                }

                attachedPrefab.name = prefab.name;

                attachedPrefab.transform.parent = transform;
                attachedPrefab.transform.localPosition = offsetSettings.position;
                attachedPrefab.transform.localRotation = offsetSettings.rotation * ((!alternateJoints) ? Quaternion.identity : Quaternion.AngleAxis(linkIndex % 2 == 0 ? 90 : 0, Vector3.up));
                attachedPrefab.transform.localScale = offsetSettings.scale;
            }
        }
        public void ApplyColliderSettings()
        {
            Collider[] cols = gameObject.GetComponents<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(cols[i]);
                else
                    GameObject.DestroyImmediate(cols[i]);
            }

            switch (colliderSettings.type)
            {
                case QuickRope.ColliderType.None:
                    collider = null;
                    break;

                case QuickRope.ColliderType.Box:
                    collider = gameObject.AddComponent<BoxCollider>();
                    BoxCollider bc = collider as BoxCollider;
                    bc.material = colliderSettings.physicsMaterial;
                    bc.size = colliderSettings.size;
                    bc.center = colliderSettings.center;
                    break;

                case QuickRope.ColliderType.Capsule:
                    collider = gameObject.AddComponent<CapsuleCollider>();
                    CapsuleCollider cc = collider as CapsuleCollider;
                    cc.material = colliderSettings.physicsMaterial;
                    cc.radius = colliderSettings.radius;
                    cc.height = colliderSettings.height;
                    cc.direction = (int)colliderSettings.direction;
                    cc.center = colliderSettings.center;
                    break;

                case QuickRope.ColliderType.Sphere:
                    collider = gameObject.AddComponent<SphereCollider>();
                    SphereCollider sc = collider as SphereCollider;
                    sc.material = colliderSettings.physicsMaterial;
                    sc.radius = colliderSettings.radius;
                    sc.center = colliderSettings.center;
                    break;
            }
        }
        public void ApplyRigidbodySettings()
        {
            if (rigidbody == null)
            {
                rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody == null)
                    rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            prevIsKinematic = rigidbodySettings.isKinematic;

            rigidbody.mass = rigidbodySettings.mass;
            rigidbody.drag = rigidbodySettings.drag;
            rigidbody.angularDrag = rigidbodySettings.angularDrag;
            rigidbody.useGravity = rigidbodySettings.useGravity;
            rigidbody.isKinematic = rigidbodySettings.isKinematic;
            rigidbody.interpolation = rigidbodySettings.interpolate;
            rigidbody.collisionDetectionMode = rigidbodySettings.collisionDetection;
            rigidbody.constraints = rigidbodySettings.constraints;

            rigidbody.solverIterationCount = rigidbodySettings.solverCount;

            if (!rigidbody.isKinematic)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.Sleep();
            }
        }
        public void ApplyJointSettings(float _anchorOffset)
        {

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
            RemoveJoint();
            joint = gameObject.AddComponent<CharacterJoint>();
#else
            if (joint == null)
            {
                joint = gameObject.GetComponent<CharacterJoint>();
                if (joint == null)
                    joint = gameObject.AddComponent<CharacterJoint>();
            }
#endif

            joint.anchor = new Vector3(0, _anchorOffset, 0);
            joint.autoConfigureConnectedAnchor = true;
            //joint.connectedAnchor = new Vector3(0, 0, 0);

            joint.axis = Vector3.up;
            joint.swingAxis = Vector3.right;

            joint.lowTwistLimit = new SoftJointLimit() { limit = -jointSettings.twistLimit };
            joint.highTwistLimit = new SoftJointLimit() { limit = jointSettings.twistLimit };

            joint.swing1Limit = new SoftJointLimit() { limit = jointSettings.swingLimit };
            joint.swing2Limit = new SoftJointLimit() { limit = jointSettings.swingLimit };
            
            joint.breakForce = jointSettings.breakForce;
            joint.breakTorque = jointSettings.breakTorque;
        }

        public void RemoveJoint()
        {
            if (joint == null)
                return;

            if (Application.isPlaying)
                GameObject.Destroy(joint);
            else
                GameObject.DestroyImmediate(joint);

            joint = null;
        }
        public void RemoveRigidbody()
        {
            if (rigidbody == null)
                return;

            RemoveJoint();

            if (Application.isPlaying)
                GameObject.Destroy(rigidbody);
            else
                GameObject.DestroyImmediate(rigidbody);

            rigidbody = null;
        }

        public GameObject AttachedPrefab()
        {
            return attachedPrefab;
        } 
        public void Destroy()
        {

            if (Application.isPlaying)
                GameObject.Destroy(gameObject);
            else
                GameObject.DestroyImmediate(gameObject);

            gameObject = null;
        }
    }
}