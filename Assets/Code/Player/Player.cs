using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Player : MonoBehaviour
{
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode move = KeyCode.W;

    public float health = 100f;
    public float respawnDuration = 3f;

    [ColorUsage(false)]
    public Color playerColor = Color.white;

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

    private Vector2 originalPosition;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

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

    public void Damage()
    {
        health -= 10f;

        if (health < 0f)
        {
            health = 0f;
        }
    }

    private void Update()
    {
        //set color
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(block);
        block.SetColor("_Color", playerColor);
        block.SetColor("_EmissionColor", playerColor * 0.5f);
        meshRenderer.SetPropertyBlock(block);

        //set rotation
        Vector3 euler = transform.localEulerAngles;
        euler.z = 0f;
        transform.localEulerAngles = euler;
    }

    private void OnGUI()
    {
        Vector3 position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        GUI.Label(new Rect(position.x, Screen.height - position.y, 200, 200), health.ToString());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Water")
        {
            Spawn();
        }
    }
}