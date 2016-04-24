/****************************************************************************************
 *   Project: QuickRopes - Chain and Rope Editing Tool (For Unity3d)
 *   File: RopeRenderer.cs
 * 
 *   Author: Jacob L. Fletcher  (mailto:reveriejake87@gmail.com)
 *   Website: http://picogames.com
 *   Documentation: http://picogames.com/quickropes-documentation/
 *   
 *   Version: 3.0.1
 *   Initial Release: May 02, 2015
 *   Latest Release: May 05, 2015
***************************************************************************************/

using UnityEngine;
using System.Collections;

using PicoGames.QuickRopes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PicoGames.Utilities
{
    [AddComponentMenu("PicoGames/QuickRopes/Extensions/Rope Renderer")]
    [ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(QuickRope))]
    public class RopeRenderer : MonoBehaviour
    {
        // Editor Variables
        public bool showBounds = false;
        public bool showEdges = false;
        public bool showNormals = false;

        // Shape Variables
        [SerializeField]
        private int leafs = 6;
        [SerializeField]
        private int detail = 1;
        [SerializeField]
        private float center = 4;

        // Strand Variables
        [SerializeField, Min(1)]
        private int strandCount = 1;
        [SerializeField]
        private float strandOffset = 0.75f;
        [SerializeField]
        private float twistAngle = 0;

        // Misc Variables
        [SerializeField]
        private float radius = 1;
        [SerializeField]
        private AnimationCurve radiusCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        [SerializeField]
        private Material material = null;
        [SerializeField]
        private Vector2 uvTile = new Vector2(1, 1);
        [SerializeField]
        private Vector2 uvOffset = new Vector2(0, 0);

        // QuickRope Reference Variable
        [SerializeField, HideInInspector]
        private QuickRope rope;

        // Construction Variables
        [SerializeField, HideInInspector]
        private bool flagShapeUpdate = true;
        [SerializeField, HideInInspector]
        private Vector3[] shapeLookup;
        [SerializeField, HideInInspector]
        private int[] shapeTriIndices;
        [SerializeField, HideInInspector]
        private int edgeCount = 0;
        [SerializeField, HideInInspector]
        private Vector3 kUp = Vector3.zero;

        [SerializeField, HideInInspector]
        private Vector3[] positions;
        [SerializeField, HideInInspector]
        private Quaternion[] rotations;
        [SerializeField, HideInInspector]
        private Vector3[] directions;

        // Mesh Variables
        [SerializeField, HideInInspector]
        private Vector3[] vertices;
        [SerializeField, HideInInspector]
        private Vector3[] normals;
        [SerializeField, HideInInspector]
        private int[] triangles;
        [SerializeField, HideInInspector]
        private Vector2[] uvs;

        private GameObject meshObject;
        private Mesh mesh;
        private MeshRenderer mRenderer;
        private MeshFilter mFilter;

        public int EdgeCount { get { return leafs; } set { if (leafs == value)return; flagShapeUpdate = true; leafs = value; } }
        public int EdgeDetail { get { return detail; } set { if (detail == value)return; flagShapeUpdate = true; detail = value; } }
        public float EdgeIndent { get { return center; } set { if (center == value)return; flagShapeUpdate = true; center = value; } }

        public int StrandCount { get { return strandCount; } set { if (strandCount == value)return; strandCount = value; } }
        public float StrandOffset { get { return strandOffset; } set { if (strandOffset == value)return; strandOffset = value; } }
        public float StrandTwist { get { return twistAngle; } set { if (twistAngle == value)return; twistAngle = value; } }
        public float Radius { get { return radius; } set { if (radius == value)return; radius = value; } }
        public AnimationCurve RadiusCurve { get { return radiusCurve; } set { radiusCurve = value; } }

        public Material Material { get { return material; } set { material = value; } }
        public Vector2 UVOffset { get { return uvOffset; } set { uvOffset = value; } }
        public Vector2 UVTile { get { return uvTile; } set { uvTile = value; } }

        void OnDrawGizmos()
        {
            if (mesh != null && showBounds)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(mesh.bounds.center, mesh.bounds.size);
            }
            
            if (vertices != null && (showEdges || showNormals))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(vertices[i], Vector3.one * 0.01f);

                    if (showNormals)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawRay(vertices[i], normals[i]);
                    }
                }
            }

            if (showEdges)
            {
                Gizmos.color = Color.blue;

                //for (int v = 0; v < vertices.Length - 1; v++)
                //    Gizmos.DrawLine(vertices[v], vertices[v + 1]);

                for(int s = 0; s < strandCount; s++)
                {
                    for(int p = 0; p < positions.Length; p++)
                    {
                        for(int e = 0; e < (edgeCount + 1); e++)
                        {
                            Gizmos.DrawLine(vertices[e + (p * (edgeCount + 1))], vertices[((e + 1) % edgeCount) + (p * (edgeCount + 1))]);
                            //Gizmos.DrawLine(vertices[e + (p * (edgeCount + 1))], vertices[((e + 1) % edgeCount) + (p * (edgeCount + 1))]);
                        }
                    }
                }
            }
        }

        void OnDestroy()
        {
            if(meshObject != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(mesh);
                    Destroy(meshObject);
                }
                else
                {
                    DestroyImmediate(mesh);
                    DestroyImmediate(meshObject);
                }
            }
        }

        void OnDisable()
        {
            if (meshObject != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(mesh);
                    Destroy(meshObject);
                }
                else
                {
                    DestroyImmediate(mesh);
                    DestroyImmediate(meshObject);
                }
            }
        }

        void Start()
        {
            rope = gameObject.GetComponent<QuickRope>();
        }

        private bool dontRedraw = false;
        public void Update()
        {
            if (Application.isPlaying && dontRedraw)
                return;

            if(flagShapeUpdate)
                UpdateShape();

            UpdatePositions();
            UpdateRotations();

            RedrawMesh();

            if (gameObject.isStatic)
                dontRedraw = true;
        }

        void VerifyMeshExists()
        {
            if (meshObject == null)
            {
                meshObject = GameObject.Find("Rope_Obj_" + gameObject.GetInstanceID());
                if(meshObject == null)
                    meshObject = new GameObject("Rope_Obj_" + gameObject.GetInstanceID(), typeof(MeshFilter), typeof(MeshRenderer));

                meshObject.hideFlags = HideFlags.HideInHierarchy;
            }

            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.hideFlags = HideFlags.DontSave;
            }

            if(mFilter == null)
            {
                mFilter = meshObject.GetComponent<MeshFilter>();
                if (mFilter == null)
                    mFilter = meshObject.AddComponent<MeshFilter>();
            }

            if(mRenderer == null)
            {
                mRenderer = meshObject.GetComponent<MeshRenderer>();
                if (mRenderer == null)
                    mRenderer = meshObject.AddComponent<MeshRenderer>();
            }

            if(material == null)
            {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
                material = new Material(Shader.Find("Diffuse"));
#else
                material = new Material(Shader.Find("Standard"));
#endif
            }

            //material.hideFlags = HideFlags.DontSave;
        }
        
        static int vertIndex = 0;
        static int triaIndex = 0;
        void RedrawMesh()
        {
            strandCount = Mathf.Max(1, strandCount);
            edgeCount = Mathf.Max(3, edgeCount);
            detail = Mathf.Max(1, detail);

            VerifyMeshExists();

            int targVertCount = ((edgeCount + 1) * positions.Length * strandCount) + (shapeLookup.Length * strandCount * 2);
            int targTriaCount = (6 * edgeCount * positions.Length * strandCount) + (shapeTriIndices.Length * strandCount * 2);

            // Resize Vertices
            if (vertices == null || vertices.Length != targVertCount)
                vertices = new Vector3[targVertCount];

            // Resize Normals
            if (normals == null || normals.Length != targVertCount)
                normals = new Vector3[targVertCount];

            if (uvs == null || uvs.Length != targVertCount)
                uvs = new Vector2[targVertCount];

            // Resize Triangles
            if (triangles == null || triangles.Length != targTriaCount)
                triangles = new int[targTriaCount];

            // Calculate Bounds
            Vector3 minBounds = Vector3.one * float.MaxValue;
            Vector3 maxBounds = Vector3.one * float.MinValue;

            // Get Vertex Placement For Tube
            vertIndex = triaIndex = 0;
            Matrix4x4 matrix = new Matrix4x4();
            for (int s = 0; s < strandCount; s++)
            {
                float radAngle = (360f / (float)strandCount) * s * Mathf.Deg2Rad;
                Vector3 rotAngle = new Vector3(Mathf.Cos(radAngle), Mathf.Sin(radAngle), 0);

                int tubeStartIndex = vertIndex;

                // Build Tube
                for (int p = 0; p < positions.Length; p++)
                {
                    float _radius = radiusCurve.Evaluate(p * (1f / (float)positions.Length)) * radius;
                    matrix.SetTRS(positions[p] + ((strandCount > 1) ? (rotations[p] * rotAngle * (_radius * strandOffset)) : Vector3.zero), rotations[p], Vector3.one * _radius);

                    for (int e = 0; e < (edgeCount + 1); e++)
                    {
                        int shapeIndex = (e % shapeLookup.Length);

                        vertices[vertIndex] = matrix.MultiplyPoint3x4(shapeLookup[shapeIndex]);
                        normals[vertIndex] = rotations[p] * shapeLookup[shapeIndex];
                        uvs[vertIndex] = new Vector2(((e / (float)edgeCount) * edgeCount) * uvTile.x + uvOffset.x, ((p / (float)(positions.Length - 1)) * positions.Length) * uvTile.y + uvOffset.y);

                        minBounds = Vector3.Min(minBounds, vertices[vertIndex]);
                        maxBounds = Vector3.Max(maxBounds, vertices[vertIndex]);

                        if(p < (positions.Length - 1) && e < edgeCount)
                        {
                            triangles[triaIndex++] = vertIndex;
                            triangles[triaIndex++] = (vertIndex + 1);
                            triangles[triaIndex++] = (vertIndex + edgeCount + 1);

                            triangles[triaIndex++] = (vertIndex + edgeCount + 1);
                            triangles[triaIndex++] = (vertIndex + 1);
                            triangles[triaIndex++] = (vertIndex + 1 + edgeCount + 1);
                        }

                        vertIndex++;
                    }
                }

                int tubeEndIndex = (vertIndex - 1);

                // Attach Caps
                for(int v = 0; v < shapeLookup.Length; v++)
                {
                    vertices[vertIndex] = vertices[tubeStartIndex + v];
                    vertices[vertIndex + shapeLookup.Length] = vertices[tubeEndIndex - v];

                    normals[vertIndex] = rotations[0] * Vector3.back;
                    normals[vertIndex + shapeLookup.Length] = rotations[rotations.Length - 1] * Vector3.forward;

                    uvs[vertIndex] = new Vector2(shapeLookup[v].x, shapeLookup[v].y);
                    uvs[vertIndex + shapeLookup.Length] = new Vector2(shapeLookup[v].x, shapeLookup[v].y);

                    vertIndex++;
                }
                vertIndex += shapeLookup.Length;

                for(int t = 0; t < shapeTriIndices.Length; t++)
                {
                    triangles[triaIndex] = (tubeEndIndex + 1) + shapeTriIndices[t];
                    triangles[triaIndex + shapeTriIndices.Length] = (tubeEndIndex + shapeLookup.Length + 1) + shapeTriIndices[t];

                    triaIndex++;
                }
                triaIndex += shapeTriIndices.Length;
            }

            mesh.Clear();
            mesh.MarkDynamic();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uvs;

            Vector3 bSize = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, maxBounds.z - minBounds.z);
            Vector3 bCenter = new Vector3(minBounds.x + bSize.x * 0.5f, minBounds.y + bSize.y * 0.5f, minBounds.z + bSize.z * 0.5f);
            mesh.bounds = new Bounds(bCenter, bSize);

            mFilter.sharedMesh = mesh;

            Material _material = material;
            mRenderer.sharedMaterial = _material;
        }

        void UpdatePositions()
        {
            int positionCount = (rope.ActiveLinkCount + 1);
            
            if (positions == null || positions.Length != positionCount)
                System.Array.Resize(ref positions, positionCount);

            for (int p = 0; p <= rope.ActiveLinkCount; p++)
                positions[p] = rope.Links[p].transform.position;

            if (rope.Spline.IsLooped)
                positions[positions.Length - 1] = positions[0];
            else
                positions[positions.Length - 1] = rope.Links[rope.Links.Length - 1].transform.position;
        }

        void UpdateShape()
        {
            shapeLookup = Shape.GetRoseCurve(leafs, detail, center, true);

            shapeTriIndices = Triangulate.Edge(shapeLookup);

            edgeCount = shapeLookup.Length;
            flagShapeUpdate = false;
        }

        void UpdateRotations()
        {
            if (rotations == null || rotations.Length != positions.Length)
                System.Array.Resize(ref rotations, positions.Length);

            if (directions == null || directions.Length != positions.Length)
                System.Array.Resize(ref directions, positions.Length);

            for (int p = 0; p < positions.Length - 1; p++)
                directions[p].Set(positions[p + 1].x - positions[p].x, positions[p + 1].y - positions[p].y, positions[p + 1].z - positions[p].z);

            directions[directions.Length - 1] = directions[directions.Length - 2];

            Vector3 forward = Vector3.zero;
            Vector3 up = (kUp == Vector3.zero) ? (directions[0].x == 0 && directions[0].z == 0 ? Vector3.right : Vector3.up) : kUp;

            for (int p = 0; p < positions.Length; p++)
            {
                if (p != 0 && p != positions.Length - 1)
                {
                    forward.Set(directions[p].x + directions[p - 1].x, directions[p].y + directions[p - 1].y, directions[p].z + directions[p - 1].z);
                }
                else
                {
                    if (positions[0] == positions[positions.Length - 1])
                        forward.Set(directions[positions.Length - 1].x + directions[0].x, directions[positions.Length - 1].y + directions[0].y, directions[positions.Length - 1].z + directions[0].z);
                    else
                        forward.Set(directions[p].x, directions[p].y, directions[p].z);
                }

                if (forward == Vector3.zero)
                {
                    rotations[p] = Quaternion.identity;
                    continue;
                }

                forward.Normalize();
                Vector3 right = Vector3.Cross(up, forward);
                up = Vector3.Cross(forward, right);

                if (p == 0)
                    kUp = up;

                if (twistAngle != 0)
                    up = Quaternion.AngleAxis(twistAngle, forward) * up;

                rotations[p].SetLookRotation(forward, up);
                //rotations[p] *= rope.Links[p].transform.rotation;
            }

            if (rope.Spline.IsLooped)
                rotations[rotations.Length - 1] = rotations[0];
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/3D Object/QuickRopes/Create: Rope At Origin", priority=11)]
        private static void CreateRopeObject()
        {
            GameObject ropeObj = new GameObject("New QuickRope");
            QuickRope rope = ropeObj.AddComponent<QuickRope>();

            rope.defaultPrefab = null;
            ropeObj.AddComponent<RopeRenderer>();

            Selection.activeGameObject = ropeObj;
        }
#endif
    }
}