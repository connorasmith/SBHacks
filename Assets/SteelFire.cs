using UnityEngine;
using System.Collections;

public class SteelFire : MonoBehaviour {

  public ParticleSystem[] explosions;
  private int explodeDelay = 1;

  private float numExplosions = 0;
  private float explosionMax = 20;

	//Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void Explode() {

    StartCoroutine(explodeTime());

  }

  IEnumerator explodeTime() {

    if(numExplosions < explosionMax) {
      for(int i = 0; i < explosions.Length; i++) {
        //syield return new WaitForSeconds(explodeDelay);

        yield return new WaitForSeconds(0.2f);
        numExplosions++;
        explosions[i].Play();
        yield return null;

      }
    }
  }
}
