using UnityEngine;
using System.Collections;

//TODO: Fix Sticky bug: Throw something too hard and the hand will still have it selected. Probably too fast for OnTriggerExit to catch it. Currently ignoring.

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class GrabScriptVive : MonoBehaviour {
  public GameObject currentlySelectedObject;

  //public Rigidbody attachPoint;

  SteamVR_TrackedObject trackedObj;
  FixedJoint joint;
  bool grabbed = false;

  Vector3 grabStartRotation;

  //The info screen attached to this hand.
  public GameObject infoScreen;

  //Keeps track of whether an object is being dragged, don't highlight things if you
  //are currently dragging.
  public bool isGrabbing = false;

  void Awake() {
    trackedObj = GetComponent<SteamVR_TrackedObject>();
  }

  void Update() {

    var device = SteamVR_Controller.Input((int)trackedObj.index);

    if(currentlySelectedObject != null) {

      if(currentlySelectedObject.GetComponent<GrabbableVive>()) {
        //Start grabbing. Update: No longer on GetTouchDown allowing for "sticky" hands. Though the sticky bug exists, with too slow collision detection.
        if(joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
          currentlySelectedObject.GetComponent<GrabbableVive>().isActive = true;

          isGrabbing = true;
          GameObject grabbedObject = currentlySelectedObject;
          grabbedObject.GetComponent<GrabbableVive>().onGrab();

          joint = grabbedObject.AddComponent<FixedJoint>();
          joint.connectedBody = this.gameObject.GetComponent<Rigidbody>();
        }

        //Stop grabbing.
        else if(joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
          Rigidbody rigidbody = Disconnect();

          //Setting throw velocities?
          var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
          if(origin != null) {
            rigidbody.velocity = origin.TransformVector(device.velocity);
            rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity) * .01f;
          }
          else {
            rigidbody.velocity = device.velocity;
            rigidbody.angularVelocity = device.angularVelocity * .01f;
          }

          rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
        }
      }

      else if( currentlySelectedObject.GetComponent<ViveTurn>()) {

        if(device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {

          if(!grabbed) {

            GameObject grabbedObject = currentlySelectedObject;
            grabbedObject.GetComponent<ViveTurn>().onGrab();
            grabbed = true;
            grabStartRotation = this.transform.localEulerAngles;

          }
        }

        else if(device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {

          grabbed = false;
          currentlySelectedObject.GetComponent<ViveTurn>().onHoverLeave();

        }

        if(grabbed) {

          Vector3 rotDelta = this.transform.localEulerAngles - grabStartRotation;
          currentlySelectedObject.GetComponent<ViveTurn>().UpdateRotation(rotDelta);
          grabStartRotation = this.transform.localEulerAngles;

        }
      }
    }
  }

  //All grabbing code is in fixed update, but this sets it up so that fixed update
  //Knows the closest object.
  void OnTriggerStay(Collider other) {

    if(other.gameObject.GetComponent<GrabbableVive>() && !isGrabbing) {

      if(currentlySelectedObject != null && other.gameObject == currentlySelectedObject) {
        currentlySelectedObject.GetComponent<GrabbableVive>().onHoverLeave();
      }

      currentlySelectedObject = other.gameObject;
      other.gameObject.GetComponent<GrabbableVive>().onHover();
    }

    else if(other.gameObject.GetComponent<ViveTurn>() && !isGrabbing) {

      currentlySelectedObject = other.gameObject;
      other.gameObject.GetComponent<ViveTurn>().onHover();

    }
  }

  //On leaving the hitbox, stop the highlight.
  void OnTriggerExit(Collider other) {
    if(other.GetComponent<GrabbableVive>() && !isGrabbing) {
      other.GetComponent<GrabbableVive>().onHoverLeave();
      if(other.gameObject == currentlySelectedObject) {
        //currentlySelectedObject.GetComponent<Grabbable>()();
        currentlySelectedObject = null;
      }
    }

    else if(other.GetComponent<ViveTurn>() && !isGrabbing) {

      other.GetComponent<ViveTurn>().onHoverLeave();

      if(other.gameObject == currentlySelectedObject) {

        currentlySelectedObject = null;
        grabbed = false;

      }
    }
  }


  /// <summary>
  /// Disconnects the object attached to this hand.
  /// </summary>
  /// <returns>The rigidbody of the previously connected object.</returns>
  public Rigidbody Disconnect() {
    currentlySelectedObject.GetComponent<GrabbableVive>().isActive = false;

    isGrabbing = false;

    //var go = joint.gameObject;
    Rigidbody rigidbody = joint.gameObject.GetComponent<Rigidbody>();
    Object.Destroy(joint);
    joint = null;
    return rigidbody;
  }
}
