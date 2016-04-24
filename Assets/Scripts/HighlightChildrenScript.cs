using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Blatantly stolen from, including shaders:
//http://nihilistdev.blogspot.com/2013/05/outline-in-unity-with-mesh-transparency.html
//Also shader here: http://answers.unity3d.com/questions/60155/is-there-a-shader-to-only-add-an-outline.html
public class HighlightChildrenScript : MonoBehaviour
{

  public bool highlightChildren = true;

    public Color meshColor = new Color(1f, 1f, 1f, 0.5f);
    public Color outlineColor = new Color(1f, 1f, 0f, 1f);

    //A list containing arrays of materials. Index of list refers to the gameObject.
    private List<Material[]> normalMaterials;
    private List<GameObject> outlineObjects;
    private List<GameObject> normalObjects;

    // Use this for initialization
    public void Start()
    {
        normalMaterials = new List<Material[]>();
        outlineObjects = new List<GameObject>();
        normalObjects = new List<GameObject>();

    // Set all the original materials into normalMaterials.
    MeshRenderer[] meshRenderers = new MeshRenderer[1];

    if (highlightChildren) {

      meshRenderers = GetComponentsInChildren<MeshRenderer>();

    }

    else {

      meshRenderers[0] = this.GetComponent<MeshRenderer>();

    }

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            normalObjects.Add(meshRenderers[i].gameObject);

            Material[] materials = meshRenderers[i].materials;

            normalMaterials.Add(new Material[materials.Length]);

            //For this mesh renderer, store all materials.
            for (int j = 0; j < materials.Length; j++)
            {
                normalMaterials[i][j] = new Material(materials[j]);
            }

            // Create a copy of this object for every child, this will have the shader that makes the real outline
            GameObject outlineObj = new GameObject();
            outlineObj.transform.parent = meshRenderers[i].gameObject.transform;

            outlineObj.transform.position = meshRenderers[i].gameObject.transform.position;
            outlineObj.transform.rotation = meshRenderers[i].gameObject.transform.rotation;
            outlineObj.transform.localScale = new Vector3(1f, 1f, 1f);
            outlineObj.AddComponent<MeshFilter>();
            outlineObj.AddComponent<MeshRenderer>();
            Mesh mesh;
            mesh = (Mesh)Instantiate(meshRenderers[i].gameObject.GetComponent<MeshFilter>().mesh);
            outlineObj.GetComponent<MeshFilter>().mesh = mesh;

            materials = new Material[materials.Length];
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j] = new Material(Shader.Find("Stencil/Outline"));

                //materials[j] = new Material(Shader.Find("Outlined/Silhouette Only"));
                //materials[j].SetColor("_OutlineColor", outlineColor);
            }
            outlineObj.GetComponent<MeshRenderer>().materials = materials;

            outlineObj.SetActive(false);

            outlineObjects.Add(outlineObj);
        }
    }

    public void makeTransparent()
    {
        // Set the transparent material to this object
        for (int i = 0; i < normalObjects.Count; i++)
        {
            Material[] materials = normalObjects[i].GetComponent<MeshRenderer>().materials;
            int materialsNum = materials.Length;
            for (int j = 0; j < materialsNum; j++)
            {
                materials[j] = new Material(Shader.Find("Outline/Transparent"));
                materials[j].SetColor("_color", meshColor);
            }

            //Activate the outline objects
            for (int j = 0; j < outlineObjects.Count; j++)
            {
                outlineObjects[j].SetActive(true);
            }
        }
    }


    public void makeOpaque()
    {
        // Set the transparent material to this object
        //MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        //Debug.Log("mLength" + normalObjects.Count);
        //Debug.Log("nLength" + normalMaterials.Count);

        for (int i = 0; i < normalObjects.Count; i++)
        {
            //Debug.Log(i);
            normalObjects[i].GetComponent<MeshRenderer>().materials = normalMaterials[i];

            //Activate the outline objects
            for (int j = 0; j < outlineObjects.Count; j++)
            {
                outlineObjects[j].SetActive(false);
            }
        }
    }
}


