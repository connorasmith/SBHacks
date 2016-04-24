using UnityEngine;
using System.Collections;

//All objects that can be grabbed need to have this.
[RequireComponent(typeof(HighlightChildrenScript), typeof(Rigidbody), typeof(Collider))]
public class GrabbableVive: MonoBehaviour {

    public bool createDialogue;
    private int numTimesPickedUp = 0;
    public string firstTouchDialogue;

    public bool isTool;
    public bool isActive;

    public float soundThreshold = .5f;

	public void onHover()
    {
        GetComponent<HighlightChildrenScript>().makeTransparent();
    }

    public void onHoverLeave()
    {
        GetComponent<HighlightChildrenScript>().makeOpaque();

    }

    public void onGrab()
    {
        numTimesPickedUp++;
        GetComponent<HighlightChildrenScript>().makeOpaque();
    }

    public void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.name);
        /*
        if (Vector3.Magnitude(GetComponent<Rigidbody>().velocity) > soundThreshold)
        {
            GetComponent<AudioSource>().Play();
        }*/
    }

    public void onRelease()
    {

    }
}
