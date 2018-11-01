using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float force = 5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponentInParent<PlayerMovement>();
        if (playerMovement)
        {
            Rigidbody rigidbody = playerMovement.GetComponent<Rigidbody>();
            rigidbody.velocity = transform.forward * force;
        }
    }
}
