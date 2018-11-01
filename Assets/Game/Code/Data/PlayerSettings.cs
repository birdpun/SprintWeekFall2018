using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSettings : ScriptableObject
{
    public float respawnDuration = 1f;
    public int lives = 5;

    public float moveSpeed = 1f;
    public float moveAcceleration = 1f;

    public float steerSpeed = 1f;
    public float steerAcceleration = 10f;
    public float steerDeceleration = 30f;

    [Header("Knockback feedback")]
    public float low = 0f;
    public float high = 1f;
    public float amp = 100f;
    public int duration = 1;
}
