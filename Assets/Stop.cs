using UnityEngine;
using System.Collections;

public class Stop : MonoBehaviour {

	// Use this for initialization
	void Start () {

    this.GetComponent<ParticleSystem>().Stop();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
