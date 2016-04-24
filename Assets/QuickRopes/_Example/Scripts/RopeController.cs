using UnityEngine;
using System.Collections;

using PicoGames.QuickRopes;

[RequireComponent(typeof(QuickRope))]
public class RopeController : MonoBehaviour
{
    [Min(1)]
    public int minJointCount = 3;
    [Min(0.001f)]
    public float maxSpeed = 5;
    [Range(0, 1f)]
    public float acceleration = 1f;
    [Range(0.001f, 1)]
    public float dampening = 1f;

    private QuickRope rope = null;

    void Awake()
    {
        rope = GetComponent<QuickRope>();

        if (rope.Spline.IsLooped)
        {
            enabled = false;
            return;
        }

        rope.minLinkCount = minJointCount;

        if (!rope.canResize)
        {
            rope.maxLinkCount = rope.Links.Length + 1;
            rope.canResize = true;

            rope.Generate();
        }

        rope.Links[rope.Links.Length - 1].Rigidbody.isKinematic = true;
    }

    void Update()
    {
        rope.velocityAccel = acceleration;
        rope.velocityDampen = dampening;

        if (Input.GetKey(KeyCode.UpArrow))
            rope.Velocity = maxSpeed;
        else if (Input.GetKey(KeyCode.DownArrow))
            rope.Velocity = -maxSpeed;
        else
            rope.Velocity = 0;
    }
}