using UnityEngine;
using System.Collections;

public class SolutionSource : MonoBehaviour {

    public Solution solutionToAdd;
    public FluidHolderScript parent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("doi");
        FluidHolderScript holder = other.GetComponent<FluidHolderScript>();
        if (holder != null && holder != parent)
        {
            other.GetComponent<FluidHolderScript>().addToSolution(solutionToAdd);
            Destroy(this.gameObject);
        }
    }
}
