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
            if (gameObject.tag == "Enviroboost")
            {
                FMODUnity.RuntimeManager.CreateInstance("event:/Boost").start();
            }
            Rigidbody rigidbody = playerMovement.GetComponent<Rigidbody>();
            rigidbody.velocity = transform.forward * force;
        }
    }
}
