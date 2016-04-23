using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HighlightChildrenScript), typeof(Rigidbody), typeof(Collider))]
public class ViveTurn : MonoBehaviour {

  public Vector3 turnAxes;

  Vector3 storedRotation;

  public bool createDialogue;
  private int numTimesPickedUp = 0;
  public string firstTouchDialogue;

  public bool isTool;
  public bool isActive;

  public float soundThreshold = .5f;

  public void onHover() {
    GetComponent<HighlightChildrenScript>().makeTransparent();
  }

  public void onHoverLeave() {
    GetComponent<HighlightChildrenScript>().makeOpaque();

  }

  public void onGrab() {
    numTimesPickedUp++;
    GetComponent<HighlightChildrenScript>().makeOpaque();

    storedRotation = this.transform.eulerAngles;

  }

  public void UpdateRotation(Vector3 delta) {


    delta.x *= turnAxes.x;
    delta.y *= turnAxes.y;
    delta.z *= turnAxes.z;

    Vector3 rotChange = storedRotation + delta;

    this.transform.eulerAngles = rotChange;

  }

  public void OnCollisionEnter(Collision other) {
    //Debug.Log(other.gameObject.name);
    /*
    if (Vector3.Magnitude(GetComponent<Rigidbody>().velocity) > soundThreshold)
    {
        GetComponent<AudioSource>().Play();
    }*/
  }

  public void onRelease() {

  }
}
