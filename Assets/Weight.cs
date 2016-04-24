using UnityEngine;
using System.Collections;

public class Weight : MonoBehaviour {

  public TextMesh weightText;
  private string displayString;
  private float weight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter(Collider other) {

    float objWeight = other.GetComponent<ObjWeight>().weight;
    weight += objWeight;
    UpdateString();

  }

  void OnTriggerExit(Collider other) {

    float objWeight = other.GetComponent<ObjWeight>().weight;
    weight -= objWeight;
    UpdateString();

  }

  void UpdateString() {

    displayString = "" + weight;
    weightText.text = displayString;


  }
}
