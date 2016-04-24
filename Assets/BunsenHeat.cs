using UnityEngine;
using System.Collections;

public class BunsenHeat : MonoBehaviour {

  public ParticleSystem bunsen;
  public Object fireBurn;
  public float heatRate = 0.1f;


  private AudioSource audio;
  public AudioClip steelAudio;
  public AudioClip paperAudio;

  bool audioPlayed = false;
  bool paperCoroutine = false;


  // Use this for initialization
  void Start () {

    audio = GetComponent<AudioSource>();

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
        if(!audioPlayed) {
          audio.clip = steelAudio;
          audio.Play();
          audioPlayed = true;
        }
        other.gameObject.GetComponent<SteelFire>().Explode();
      }
    }

    else if (other.tag == "Paper") {

      if (!paperCoroutine)
        StartCoroutine(BurnPaper(other));

    }

    else if (other.GetComponent<FluidHolderScript>()) {


      
    }
  }

  IEnumerator BurnPaper(Collider other) {

    paperCoroutine = true;

    audio.clip = paperAudio;
    audio.Play();

    ((GameObject)(GameObject.Instantiate(fireBurn, other.transform.position, Quaternion.identity))).transform.parent = other.transform;

    yield return new WaitForSeconds(3.0f);

    other.GetComponent<Joint>().connectedBody.GetComponent<GrabScriptVive>().Disconnect();

    GameObject.Destroy(other.gameObject);

    paperCoroutine = false;

  }
}
