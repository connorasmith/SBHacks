using UnityEngine;
using System.Collections;

public class SinMovement : MonoBehaviour 
{
    public float speed = 10;
    public float magnitude = 5;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = (Vector3.forward * Mathf.Sin(Time.time * speed) * magnitude) + startPosition;
    }
}
