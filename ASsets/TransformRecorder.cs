using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class TransformRecorder : MonoBehaviour
{

    public GameObject head;
    public GameObject leftHand;
    public GameObject rightHand;
    private StreamWriter file;

    // Use this for initialization
    void Start()
    {
        file = new StreamWriter("data.txt");
        file.WriteLine("Time| Head x y z rotx roty rotz| Left Hand x y z rotx roty rotz | Right Hand x y z rotx roty rotz");

        StartCoroutine(RecordData());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RecordData()
    {
        while (true)
        {
            file.WriteLine(Time.time + " | " + head.transform.position + " " + head.transform.rotation.eulerAngles + " | " +
            leftHand.transform.position + " " + leftHand.transform.rotation.eulerAngles + " | " +
            rightHand.transform.position + " " + rightHand.transform.rotation.eulerAngles);
            yield return new WaitForSeconds(.1f);
        }
    }
}