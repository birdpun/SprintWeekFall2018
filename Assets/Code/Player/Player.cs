using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode move = KeyCode.W;

    public float health = 100f;
    public float respawnDuration = 3f;

    public bool IsOnRink
    {
        get
        {
            Transform rink = GameObject.Find("Rink").transform;
            float rinkRadius = rink.localScale.x * 0.5f;
            float squaredDistance = (new Vector2(rink.position.x, rink.position.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude;
            if (squaredDistance < rinkRadius * rinkRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public float Steer
    {
        get
        {
            float steer = 0f;

            if (Input.GetKey(left)) steer -= 1;
            if (Input.GetKey(right)) steer += 1;

            return steer;
        }
    }

    public bool Move
    {
        get
        {
            return Input.GetKey(move);
        }
    }

    private float timeOutsideTheRink;
    private Vector2 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;

        Spawn();
    }

    private void Spawn()
    {
        //reset health
        health = 100f;

        //reset position and rotation
        transform.position = originalPosition;
        transform.localEulerAngles = Vector3.zero;

        //reset velocity
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (IsOnRink)
        {
            timeOutsideTheRink = 0f;
        }
        else
        {
            timeOutsideTheRink += Time.deltaTime;
        }

        //if the player was outside the rink for longer than 3 seconds, then respawn
        if (timeOutsideTheRink > respawnDuration)
        {
            Spawn();
        }
    }

    public void Damage()
    {
        health -= 10f;

        if (health < 0f)
        {
            health = 0f;
        }
    }

    private void OnGUI()
    {
        Vector3 position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        GUI.Label(new Rect(position.x, Screen.height - position.y, 200, 200), health.ToString());
    }
}