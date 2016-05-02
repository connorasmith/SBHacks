using UnityEngine;
using System.Collections;

public class SolutionSource : MonoBehaviour {

  public Solution solutionToAdd;

  public FluidHolderScript creator;

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  void OnTriggerEnter(Collider other) {
    FluidHolderScript holder = other.GetComponent<FluidHolderScript>();
    if(holder != null) {

      if(holder != creator) {
        other.GetComponent<FluidHolderScript>().addToSolution(solutionToAdd);
        Destroy(this.gameObject);
      }
    }

    else if (!other.GetComponent<Collider>().isTrigger)
    {
      Destroy(this.gameObject);
    }

  }

}
