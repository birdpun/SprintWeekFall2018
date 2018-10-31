using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask layerMask;

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
        float lerp = isSteering ? player.settings.steerAcceleration : player.settings.steerDeceleration;

        //lerp
        steer = Mathf.Lerp(steer, player.Steer, Time.deltaTime * lerp);

        //slowly lerp the movement amount
        bool wantsToMove = true;// player.Move;
        lerp = wantsToMove ? player.settings.moveAcceleration : 10000f;
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.AddForce(Physics.gravity);
        }

        Steer();
        if (isGrounded) Move();
    }

    private void Steer()
    {
        //rotate
        transform.Rotate(Vector3.up, steer * player.settings.steerSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (GameManager.State == GameState.Starting)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        //if (!player.Move && isGrounded)
        //{
        //    Vector3 velocity = rb.velocity;
        //    velocity *= (1f - deceleration);
        //    velocity.y = rb.velocity.y;
        //    rb.velocity = velocity;
        //    return;
        //}

        float speed = player.settings.moveSpeed - new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude * player.settings.moveAcceleration;
        rb.AddForce(transform.forward * speed);
        //transform.Translate(Vector3.forward * move * Time.deltaTime);
    }
}