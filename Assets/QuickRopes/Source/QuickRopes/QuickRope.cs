/****************************************************************************************
 *   Project: QuickRopes - Chain and Rope Editing Tool (For Unity3d)
 *   File: QuickRopes.cs
 * 
 *   Author: Jacob L. Fletcher  (mailto:reveriejake87@gmail.com)
 *   Website: http://picogames.com
 *   Documentation: http://picogames.com/quickropes-documentation/
 *   
 *   Version: 3.0.1
 *   Initial Release: May 02, 2015
 *   Latest Release: May 16, 2015
***************************************************************************************/

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

using PicoGames.Utilities;

namespace PicoGames.QuickRopes
{
    [AddComponentMenu("PicoGames/QuickRopes/QuickRope")]
    [DisallowMultipleComponent, RequireComponent(typeof(Spline))]
    public class QuickRope : MonoBehaviour
    {
        const int MAX_JOINT_COUNT = 128;
        const float MIN_JOINT_SCALE = 0.001f;
        const float MIN_JOINT_SPACING = 0.001f;

        const string VERSION = "3.1.6";

        public enum RenderType
        {
            Prefab = 0,
            Rendered = 1
        }

        public enum ColliderType
        {
            None,
            Sphere,
            Box,
            Capsule
        }

        [SerializeField, Min(MIN_JOINT_SCALE)]
        public float linkScale = 1;
        [SerializeField, Min(MIN_JOINT_SPACING)]
        public float linkSpacing = 0.5f;
        [SerializeField, Min(3)]
        public int maxLinkCount = 50;
        [SerializeField, Min(1)]
        public int minLinkCount = 1;

        [SerializeField]
        public bool alternateRotation = true;
        [SerializeField]
        public bool usePhysics = true;
        [SerializeField]
        public bool canResize = false;

        [SerializeField]
        public GameObject defaultPrefab = null;
        [SerializeField]
        public TransformSettings defaultPrefabOffsets = new TransformSettings() { position = new Vector3(0, 0.25f, 0), scale = Vector3.one };
        [SerializeField]
        public RigidbodySettings defaultRigidbodySettings = new RigidbodySettings() { mass = 1, drag = 0.1f, angularDrag = 0.05f, useGravity = true, isKinematic = false, solverCount = 6 };
        [SerializeField]
        public JointSettings defaultJointSettings = new JointSettings() { breakForce = Mathf.Infinity, breakTorque = Mathf.Infinity, swingLimit = 90, twistLimit = 40 };
        [SerializeField]
        public ColliderSettings defaultColliderSettings = new ColliderSettings() { size = Vector3.one, height = 2, center = Vector3.zero, radius = 1 };
        [SerializeField]
        public float velocityAccel = 1f;
        [SerializeField]
        public float velocityDampen = 0.98f;

        [SerializeField, HideInInspector]
        private float velocity = 0;
        [SerializeField, HideInInspector]
        private float kVelocity = 0f;
        [SerializeField, HideInInspector]
        private int activeLinkCount = 0;
        [SerializeField]
        private Link[] links = new Link[0];
        [SerializeField, HideInInspector]
        private Spline spline;
        
        public Spline Spline 
        {
            get 
            {
                if (spline == null)
                {
                    spline = gameObject.GetComponent<Spline>();
                    if (spline == null)
                    {
                        spline = gameObject.AddComponent<Spline>();
                        spline.Reset();
                    }
                }

                return spline; 
            } 
        }

        public int ActiveLinkCount { get { return activeLinkCount; } }
        public Link[] Links { get { return links; } }
        public float Velocity { get { return velocity; } set { velocity = value; } }

        //void OnDrawGizmos()
        //{
        //    SplinePoint[] initialPositions = Spline.GetSpacedPointsReversed(linkSpacing);
        //    for(int i = 0; i < initialPositions.Length; i++)
        //    {
        //        Gizmos.DrawWireCube(transform.TransformPoint(initialPositions[i].position), Vector3.one * 0.1f);
        //    }
        //}

        void Reset()
        {
            if (Application.isPlaying)
                return;

            ClearLinks();
            Spline.Reset();
        }

        public void Generate()
        {
            if (spline.IsLooped)
                canResize = false;

            SplinePoint[] splinePoints = ResizeLinkArray();

            if (splinePoints.Length == 0)
                return;

            Rigidbody previousBody = null;
            for (int i = (links.Length - 1); i >= 0; i--)
            {
                // Setup General Settings
                links[i].IsActive = (i < activeLinkCount || i == links.Length - 1);
                links[i].transform.localScale = Vector3.one * linkScale;

                links[i].gameObject.layer = gameObject.layer;
                links[i].gameObject.tag = gameObject.tag;

                if (i < (splinePoints.Length - 1))
                {
                    links[i].transform.rotation = transform.rotation * splinePoints[i].rotation;
                    links[i].transform.position = transform.TransformPoint(splinePoints[i].position);
                }
                else if (i == links.Length - 1)
                {
                    links[i].transform.rotation = transform.rotation * splinePoints[splinePoints.Length - 1].rotation;
                    links[i].transform.position = transform.TransformPoint(splinePoints[splinePoints.Length - 1].position);
                }

                // Setup Prefab Settings
                if (i != (links.Length - 1))
                {
                    if (!links[i].overridePrefab)
                        links[i].prefab = defaultPrefab;

                    if (!links[i].overrideOffsetSettings)
                        links[i].offsetSettings = defaultPrefabOffsets;

                    if (links[i].AttachedPrefab() != null)
                        links[i].AttachedPrefab().hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
                }
                links[i].alternateJoints = alternateRotation;
                links[i].ApplyPrefabSettings();

                // Setup Physics Settings
                if (!links[i].overrideRigidbodySettings)
                    links[i].rigidbodySettings = defaultRigidbodySettings;

                links[i].ApplyRigidbodySettings();

                if (i != (links.Length - 1))
                {
                    if (!links[i].overrideJointSettings)
                        links[i].jointSettings = defaultJointSettings;

                    links[i].ApplyJointSettings(linkSpacing * (1f / linkScale));
                }

                // Setup Collider Settings
                if (!links[i].overrideColliderSettings)
                    links[i].colliderSettings = defaultColliderSettings;
                links[i].ApplyColliderSettings();

                // Setup Physics Enabled
                if (links[i].TogglePhysicsEnabled(usePhysics))
                {
                    links[i].ConnectedBody = previousBody;
                    previousBody = links[i].Rigidbody;
                }
            }

            if (usePhysics)
            {                
                if (spline.IsLooped)
                {
                    links[links.Length - 1].IsActive = false;
                    activeLinkCount--;

                    links[links.Length - 2].ConnectedBody = links[0].Rigidbody;
                }
                else
                {
                    links[links.Length - 1].RemoveJoint();
                    links[links.Length - 1].IsPrefabActive = false;

                    if (canResize && (activeLinkCount != links.Length))
                        links[activeLinkCount - 1].ConnectedBody = links[links.Length - 1].Rigidbody;
                }
            }
        }

        private SplinePoint[] ResizeLinkArray()
        {
            maxLinkCount = Mathf.Min(maxLinkCount, MAX_JOINT_COUNT);
            SplinePoint[] initialPositions = Spline.GetSpacedPointsReversed(linkSpacing);
            activeLinkCount = Mathf.Min(maxLinkCount, initialPositions.Length) - 1;
            int spawnJointCount = (canResize) ? maxLinkCount : initialPositions.Length;

            if (spawnJointCount <= 0 && links.Length > 0)
            {
                for (int i = 0; i < links.Length; i++)
                    links[i].Destroy();

                links = new Link[0];
                initialPositions = new SplinePoint[0];
            }
            else
            {
                if (links.Length != spawnJointCount)
                {
                    if (spawnJointCount > links.Length)
                    {
                        int tSize = links.Length;
                        System.Array.Resize(ref links, spawnJointCount);
                        for (int i = tSize; i < spawnJointCount; i++)
                        {
                            links[i] = new Link(new GameObject("Link_[" + i + "]"), i);
                            links[i].Parent = transform;
                        }
                    }
                    else if (spawnJointCount > 0)
                    {
                        //targetJointCount -= 1;
                        for (int i = links.Length - 1; i >= spawnJointCount; i--)
                            links[i].Destroy();

                        System.Array.Resize(ref links, spawnJointCount);
                    }
                }
            }

            return initialPositions;
        }

        private void ClearLinks()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Cannot destroy links whilst in 'Play' mode.");
                return;
            }

            while (transform.childCount > 0)
            {
                if (Application.isPlaying)
                    Destroy(transform.GetChild(0).gameObject);
                else
                    DestroyImmediate(transform.GetChild(0).gameObject);
            }

            links = new Link[0];
        }
        
        #region Rope Controller

        void FixedUpdate()
        {
            if (velocity != 0)
                kVelocity = Mathf.Lerp(kVelocity, velocity, Time.deltaTime * velocityAccel);
            else
                kVelocity = Mathf.Lerp(kVelocity, velocity, Time.deltaTime * velocityDampen);

            if (kVelocity > 0)
                kVelocity = Extend(kVelocity) ? kVelocity : 0;

            if (kVelocity < 0)
                kVelocity = Retract(kVelocity, minLinkCount) ? kVelocity : 0;
        }

        public bool Extend(float _speed)
        {
            if (!canResize)
                return false;

            Link rootLink = links[links.Length - 1];
            Link controlLink = links[activeLinkCount - 1];
            Vector3 targetPosition = rootLink.transform.position - (controlLink.transform.up * linkSpacing * 2);
                                                
            if (activeLinkCount < (maxLinkCount - 1))
            {
                controlLink.ConnectedBody = null;
                controlLink.transform.position = Vector3.MoveTowards(controlLink.transform.position, targetPosition, Mathf.Abs(_speed) * Time.deltaTime);

                if (Vector3.SqrMagnitude(controlLink.transform.position - rootLink.transform.position) > (linkSpacing * linkSpacing))
                {
                    Link addedLink = links[activeLinkCount];

                    addedLink.transform.position = controlLink.transform.position + ((rootLink.transform.position - controlLink.transform.position).normalized * linkSpacing);
                    addedLink.transform.rotation = controlLink.transform.rotation;

                    activeLinkCount++;

                    //addedLink.Joint.anchor = new Vector3(0, linkSpacing * (1f / linkScale), 0);
                    addedLink.IsActive = true;

                    controlLink.ConnectedBody = addedLink.Rigidbody;
                    controlLink = addedLink;
                }

                //controlLink.Joint.anchor = new Vector3(0, Vector3.Distance(rootLink.transform.position, controlLink.transform.position) * (1f / linkScale), 0);
                controlLink.ApplyJointSettings(Vector3.Distance(rootLink.transform.position, controlLink.transform.position) * (1f / linkScale));
                controlLink.ConnectedBody = rootLink.Rigidbody;
            }
            else
            {
                kVelocity = 0;
            }
                        
            return true;
        }

        public bool Retract(float _speed, int _minJointCount)
        {
            if (!canResize)
                return false;
            
            Link rootLink = links[links.Length - 1];
            Link controlLink = links[activeLinkCount - 1];

            controlLink.ConnectedBody = null;
            controlLink.transform.position = Vector3.MoveTowards(controlLink.transform.position, rootLink.transform.position, Mathf.Abs(_speed) * Time.deltaTime);

            if (activeLinkCount > _minJointCount)
            {
                if (Vector3.SqrMagnitude(rootLink.transform.position - controlLink.transform.position) <= 0.01f)
                {
                    controlLink.IsActive = false;
                    activeLinkCount--;

                    controlLink = links[activeLinkCount - 1];

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
                    controlLink.RemoveJoint();
                    controlLink.ApplyJointSettings(Vector3.Distance(rootLink.transform.position, controlLink.transform.position) * (1f / linkScale));
#endif

                }
            }
            else
            {
                kVelocity = 0;
                controlLink.transform.position = rootLink.transform.position - (controlLink.transform.up * linkSpacing);
            }

            controlLink.Joint.anchor = new Vector3(0, Vector3.Distance(rootLink.transform.position, controlLink.transform.position) * (1f / linkScale), 0);
            controlLink.ConnectedBody = rootLink.Rigidbody;

            return true;
        }

        #endregion

        public static QuickRope Create(Vector3 _position, GameObject _prefab, float _linkSpacing = 1.0f, float _prefabScale = 0.5f) { return Create(_position, _position + new Vector3(0, 5, 0), _prefab, _linkSpacing, _prefabScale); }
        public static QuickRope Create(Vector3 _pointA, Vector3 _pointB, GameObject _prefab, float _linkSpacing = 1.0f, float _prefabScale = 0.5f)
        {
            GameObject ropeObj = new GameObject("Rope");
            ropeObj.transform.position = _pointA;

            QuickRope rope = ropeObj.AddComponent<QuickRope>();
            rope.defaultPrefab = _prefab;

            Vector3 direction = (_pointB - _pointA).normalized;

            rope.Spline.SetControlPoint(1, _pointB, Space.World);
            rope.Spline.SetPoint(1, _pointA + direction, Space.World);
            rope.Spline.SetPoint(2, _pointB - direction, Space.World);

            rope.Generate();
            //rope.Generate();

            return rope;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/3D Object/QuickRopes/Create: Chain At Origin", priority=10)]
        private static void CreateRopeObject()
        {
            GameObject ropeObj = new GameObject("New QuickRope");
            ropeObj.AddComponent<QuickRope>();

            Selection.activeGameObject = ropeObj;
        }

        [MenuItem("GameObject/3D Object/QuickRopes/Documentation", priority=40)]
        private static void OpenDocumentation()
        {
            Application.OpenURL("http://www.picogames.com/quickropes-documentation/");
        }
        [MenuItem("GameObject/3D Object/QuickRopes/Developer Website", priority = 41)]
        private static void OpenDeveloperSite()
        {
            Application.OpenURL("http://www.picogames.com/");
        }
        [MenuItem("GameObject/3D Object/QuickRopes/Developer Contact", priority = 42)]
        private static void OpenDeveloperHelp()
        {
            Application.OpenURL("http://www.picogames.com/contact/");
        }
#endif
    }
}