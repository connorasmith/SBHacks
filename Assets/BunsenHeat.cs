using UnityEngine;
using System.Collections;

public class BunsenHeat : MonoBehaviour {

  public ParticleSystem bunsen;
  public Object fireBurn;
  public float heatRate = 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerStay(Collider other) {

    if(other.gameObject.GetComponent<FluidHolderScript>()) {

      FluidHolderScript fluid = other.gameObject.GetComponent<FluidHolderScript>();
      Debug.Log(bunsen.startSpeed * heatRate);
      fluid.solution.temperature += (bunsen.startSpeed * heatRate);

      if(fluid.solution.temperature > 200) {

        fluid.solution.temperature = 200;

      }
    }

    else if(other.gameObject.GetComponent<SteelFire>()) {

      if(bunsen.startSpeed >= 5) {
        other.gameObject.GetComponent<SteelFire>().Explode();
      }
    }

    else if (other.tag == "Paper") {

      StartCoroutine(BurnPaper(other));

    }
  }

  IEnumerator BurnPaper(Collider other) {

    ((GameObject)(GameObject.Instantiate(fireBurn, other.transform.position, Quaternion.identity))).transform.parent = other.transform;

    yield return new WaitForSeconds(3.0f);

    other.GetComponent<Joint>().connectedBody.GetComponent<GrabScriptVive>().Disconnect();

    GameObject.Destroy(other.gameObject);

  }
}
