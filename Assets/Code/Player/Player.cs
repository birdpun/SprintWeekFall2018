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
    public int lives = 0;

    [ColorUsage(false)]
    public Color playerColor = Color.white;

    public float Steer
    {
        get
        {
            float steer = 0f;
            if (JoyconManager.Joycons.Count > joyconIndex)
            {
                steer = (90f - JoyconManager.Joycons[joyconIndex].Euler.z) / 180f;
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

            steer = 0f;
            if (Input.GetKey(KeyCode.A)) steer -= 1f;
            if (Input.GetKey(KeyCode.D)) steer += 1f;

            if (joyconIndex == 1)
            {
                steer = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) steer -= 1f;
                if (Input.GetKey(KeyCode.RightArrow)) steer += 1f;
            }

            return steer;
        }
    }

    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        originalPosition = transform.position;
        originalRotation = transform.eulerAngles;

        Reset();
    }

    public void Reset()
    {
        lives = settings.lives;
        Spawn();
    }

    private void Spawn()
    {
        //dont respawn if celebrating
        if (GameManager.State == GameState.Celebrating) return;

        //reset position and rotation
        transform.position = originalPosition;
        transform.localEulerAngles = originalRotation;

        //reset velocity
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;

        //dont subtract lives if not in play mode
        if (GameManager.State != GameState.Playing) return;

        lives--;

        //this player ran out of lives
        //end the game
        if (lives == 0)
        {
            //find the player with the most lives
            int minLives = int.MinValue;
            Player winner = null;
            Player[] allPlayers = FindObjectsOfType<Player>();
            for (int i = 0; i < allPlayers.Length; i++)
            {
                if (allPlayers[i].lives > minLives)
                {
                    minLives = allPlayers[i].lives;
                    winner = allPlayers[i];
                }
            }

            GameManager.Celebrate(winner);
            return;
        }
    }

    public void ShakeSingle()
    {
        if (GameManager.State != GameState.Playing) return;

        if (JoyconManager.Joycons.Count > joyconIndex)
        {
            var joycon = JoyconManager.Joycons[joyconIndex];
            joycon.SetRumble(settings.low, settings.high, settings.amp, 300);
        }
    }

    public async void Shake()
    {
        if (GameManager.State != GameState.Playing) return;

        if (JoyconManager.Joycons.Count > joyconIndex)
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

        if (Application.isPlaying)
        {
            if (GameManager.State == GameState.Starting)
            {
                transform.position = originalPosition;
                transform.localEulerAngles = originalRotation;
            }

            SetRotation();
        }
    }

    private async void OnTriggerEnter(Collider other)
    {
        if (GameManager.State == GameState.Starting) return;

        //if hit water, wait 1 seconds and then respawn
        if (other.name == "Water")
        {
            Shake();
            int ms = Mathf.RoundToInt(settings.respawnDuration * 1000f);
            await Task.Delay(ms);

            //this check happens when exiting to edit mode
            if (!this) return;

            Spawn();
        }
    }
}