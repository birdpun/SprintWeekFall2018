using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuoyancy : MonoBehaviour
{
    //private comment
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //if the object is below the water level
        //add upwards force
        if (transform.position.y < 0)
        {
            rb.AddForce(Physics.gravity * -1.5f);
        }
    }
}