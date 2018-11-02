using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    public float maxDistance = 0.5f;
    public float knockbackMultiplier = 1f;
    
    private float nextKnockback;

    private void Update()
    {
        if (Time.time > nextKnockback)
        {
            RaycastHit leftHit = new RaycastHit();
            RaycastHit middleHit = new RaycastHit();
            RaycastHit rightHit = new RaycastHit();

            Vector3 leftOrigin = transform.position + transform.TransformDirection(Vector3.left * 0.5f);
            Vector3 rightOrigin = transform.position + transform.TransformDirection(Vector3.right * 0.5f);

            bool left = Physics.Raycast(leftOrigin, transform.forward, out leftHit, maxDistance);
            bool middle = Physics.Raycast(transform.position, transform.forward, out middleHit, maxDistance);
            bool right = Physics.Raycast(rightOrigin, transform.forward, out rightHit, maxDistance);

            if (left || middle || right)
            {
                Player player = null;

                //check middle
                if (middleHit.transform) player = middleHit.transform.GetComponent<Player>();

                //check left
                if (!player && leftHit.transform) player = leftHit.transform.GetComponent<Player>();

                //check right
                if (!player && rightHit.transform) player = rightHit.transform.GetComponent<Player>();

                if (player && player.transform != transform)
                {
                    Knockback(player);
                    nextKnockback = Time.time + 1f;
                }
            }
        }
    }

    private void Knockback(Player player)
    {
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        float speed = GetComponent<Rigidbody>().velocity.magnitude;

        Vector3 direction = (player.transform.position - transform.position).normalized * speed * knockbackMultiplier;
        rigidbody.velocity = direction;

        player.Shake();
        FMODUnity.RuntimeManager.CreateInstance("event:/Bump").start();

        //ask the camera to shake as well
        CameraManager.Shake();
    }
}
