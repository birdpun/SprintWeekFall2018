using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
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
            float steer = (90f - JoyconManager.Joycons[joyconIndex].Euler.z) / 180f;
            steer = Mathf.Clamp(steer * 5f, -1f, 1f);
            return steer;
        }
    }

    private Vector2 originalPosition;
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

    private void Update()
    {
        SetColor();

        //only process rotation if grounded
        if (movement.Grounded) SetRotation();
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

    public float low = 0f;
    public float high = 1f;
    public float amp = 100f;
    public int duration = 1;

    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Beat();
        }
    }

    private async void Beat()
    {
        var joycon = JoyconManager.Joycons;

        //joycon.SetRumble(low, high, amp, duration);

        await Task.Delay(duration);
        await Task.Delay(30);

        //joycon.SetRumble(low, high, amp, duration);

        await Task.Delay(duration);
        await Task.Delay(30);

        //joycon.SetRumble(low, high, amp, duration);
    }
}