using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask layerMask;

    public float moveSpeed = 1f;
    public float steerSpeed = 1f;
    public float acceleration = 1f;
    public float deceleration = 0.15f;

    private float steer = 0f;
    private float move = 0f;

    private bool wasOnRink;
    private bool isGrounded;

    private Player player;
    private Rigidbody rb;

    public bool Grounded
    {
        get
        {
            return isGrounded;
        }
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f, layerMask);

        //cache the inputs

        //slowly lerp the steer amount towards the desired steer
        bool isSteering = player.Steer != 0;
        float lerp = isSteering ? acceleration : 10000f;

        //lerp
        steer = Mathf.Lerp(steer, player.Steer, Time.deltaTime * lerp);

        //slowly lerp the movement amount
        bool wantsToMove = player.Move;
        lerp = wantsToMove ? acceleration : 10000f;
        float speed = wantsToMove ? moveSpeed : 0;

        //lerp
        move = Mathf.Lerp(move, speed, Time.deltaTime * lerp);
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.AddForce(Physics.gravity);
        }

        Steer();
        if (isGrounded) Move();

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
        if (!player.Move && isGrounded)
        {
            Vector3 velocity = rb.velocity;
            velocity *= (1f - deceleration);
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
            return;
        }

        rb.AddForce(transform.forward * move);
        //transform.Translate(Vector3.forward * move * Time.deltaTime);
    }
}