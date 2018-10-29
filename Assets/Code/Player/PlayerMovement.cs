using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float steerSpeed = 1f;
    public float acceleration = 1f;
    public float deceleration = 1f;

    private float steer = 0f;
    private float move = 0f;

    private bool wasOnRink;

    private Player player;
    private Rigidbody rb;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //cache the inputs

        //slowly lerp the steer amount towards the desired steer
        bool isSteering = player.Steer != 0;
        float lerp = isSteering ? acceleration : deceleration;

        //lerp
        steer = Mathf.Lerp(steer, player.Steer, Time.deltaTime * lerp);

        //slowly lerp the movement amount
        bool wantsToMove = player.Move;
        lerp = wantsToMove ? acceleration : deceleration;
        float speed = wantsToMove ? moveSpeed : 0;

        //lerp
        move = Mathf.Lerp(move, speed, Time.deltaTime * lerp);
    }

    private void FixedUpdate()
    {
        Steer();
        Move();

        //if the player is on the rink, ensure that the player can only rotate on the y axis
        if (player.IsOnRink)
        {
            wasOnRink = true;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            //if the player is off the rink, let the player rotate freely
            //rb.constraints = RigidbodyConstraints.None;

            if (wasOnRink)
            {
                //the player just felloff the rink
                //add some angular velocity for effect
                wasOnRink = false;

                rb.angularVelocity = rb.velocity;
            }
        }
    }

    private void Steer()
    {
        //rotate
        transform.Rotate(Vector3.up, steer * steerSpeed * Time.deltaTime);
    }

    private void Move()
    {
        //if the player is off the rink, then apply drag
        if (!player.IsOnRink)
        {
            Vector3 velocity = rb.velocity;
            velocity *= 0.75f;
            velocity.y = rb.velocity.y;
            //rb.velocity = velocity;
            return;
        }

        rb.AddForce(transform.forward * move);
        //transform.Translate(Vector3.forward * move * Time.deltaTime);
    }
}