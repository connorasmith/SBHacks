using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour {

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if(Input.GetKeyDown("r")) {
      SceneManager.LoadScene(0);
    }
  }
}