using UnityEngine;
using System.Collections;

public class BunsenHeat : MonoBehaviour {

  public ParticleSystem bunsen;
  public float heatRate = 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerStay(Collider other) {

    if (other.gameObject.GetComponent<FluidHolderScript>()) {

      FluidHolderScript fluid = other.gameObject.GetComponent<FluidHolderScript>();
      Debug.Log(bunsen.startSpeed * heatRate);
      fluid.solution.temperature += (bunsen.startSpeed * heatRate);

      if (fluid.solution.temperature > 200) {

        fluid.solution.temperature = 200;

      }
    }
  }
}
