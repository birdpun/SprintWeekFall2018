﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSettings settings;

    public int joyconIndex = 0;

    [ColorUsage(false)]
    public Color playerColor = Color.white;

    public float Steer
    {
        get
        {
            if (JoyconManager.Joycons.Count > joyconIndex)
            {
                float steer = (90f - JoyconManager.Joycons[joyconIndex].Euler.z) / 180f;
                float abs = Mathf.Abs(steer);
                if (abs < 0.04f)
                {
                    steer = 0f;
                }
                else
                {
                    if (abs > 0.15f)
                    {
                        steer = Mathf.Sign(steer);
                    }
                    else
                    {
                        steer = Mathf.Sign(steer) * 0.5f;
                    }
                }

                return steer;
            }
            else
            {
                float steer = 0f;
                if (Input.GetKey(KeyCode.A)) steer -= 1f;
                if (Input.GetKey(KeyCode.D)) steer += 1f;

                if (joyconIndex == 1)
                {
                    if (Input.GetKey(KeyCode.LeftArrow)) steer -= 1f;
                    if (Input.GetKey(KeyCode.RightArrow)) steer += 1f;
                }

                return steer;
            }
        }
    }

    private Vector3 originalPosition;
    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();

        originalPosition = transform.position;

        Spawn();
    }

    private void Spawn()
    {
        //reset position and rotation
        transform.position = originalPosition;
        transform.localEulerAngles = Vector3.zero;

        //reset velocity
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;
    }

    public async void Knocked()
    {
        var joycon = JoyconManager.Joycons[joyconIndex];

        joycon.SetRumble(settings.low, settings.high, settings.amp, settings.duration);

        await Task.Delay(settings.duration);
        await Task.Delay(30);

        joycon.SetRumble(settings.low, settings.high, settings.amp, settings.duration);

        await Task.Delay(settings.duration);
        await Task.Delay(30);

        joycon.SetRumble(settings.low, settings.high, settings.amp, settings.duration);
    }

    private void SetColor()
    {
        //change color of material without instancing a new one

        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(block);
        block.SetColor("_Color", playerColor);
        block.SetColor("_EmissionColor", playerColor * 0.5f);
        meshRenderer.SetPropertyBlock(block);
    }

    private void SetRotation()
    {
        //force x rotation to be 0

        Vector3 euler = transform.localEulerAngles;
        euler.z = 0f;
        transform.localEulerAngles = euler;
    }

    [ExecuteInEditMode]
    private void Update()
    {
        SetColor();

        if (Application.isPlaying)
        {
            //only process rotation if grounded
            if (movement.Grounded) SetRotation();
        }
    }

    private async void OnTriggerEnter(Collider other)
    {
        //if hit water, wait 1 seconds and then respawn
        if (other.name == "Water")
        {
            int ms = Mathf.RoundToInt(settings.respawnDuration * 1000f);
            await Task.Delay(ms);

            //this check happens when exiting to edit mode
            if (!this) return;

            Spawn();
        }
    }
}