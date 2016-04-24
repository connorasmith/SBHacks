using UnityEngine;
using System.Collections;

public class WaterFill : MonoBehaviour {

  public Object waterSource;
  public ParticleSystem waterSystem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter(Collider other) {

    if (other.GetComponent<FluidHolderScript>()) {
      if(waterSystem.emissionRate >= 5)
        //Debug.Log("FILLING");
      StartCoroutine(fill());

    }
  }

  void OnTriggerExit(Collider other) {

    StopAllCoroutines();

  }

  IEnumerator fill() {

    while(true) {

      //Debug.Log("COROUTINE");
      GameObject.Instantiate(waterSource, this.transform.position, Quaternion.identity);

      yield return new WaitForSeconds(0.1f);
    }

  }
}
