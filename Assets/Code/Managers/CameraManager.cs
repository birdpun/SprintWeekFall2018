using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blur = SuperBlur.SuperBlur;

[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    public float fov = 15f;
    public float shakeSettle = 10f;

    [ClampedCurve]
    public AnimationCurve startingPlayingCurve;

    [ClampedCurve]
    public AnimationCurve focusOnWinner;

    private Blur blur;
    private float shake;
    private Camera cam;
    private float startingTime;
    private float celebratingTime;
    private GameState lastState;

    public static Transform Transform
    {
        get
        {
            if (!instance) instance = FindObjectOfType<CameraManager>();

            return instance.cam.transform;
        }
    }

    private void Awake()
    {
        cam = FindObjectOfType<Camera>();
        blur = cam.GetComponent<Blur>();
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        cam.fieldOfView = fov;

        if (!Application.isPlaying) return;

        Vector3 waitingPosition = new Vector3(0f, 60f, -140f);
        Vector3 playingPosition = new Vector3(0f, 60f, -70f);
        Vector3 waitingRotation = new Vector3(21f, 0f, 0f);
        Vector3 playingRotation = new Vector3(40f, 0f, 0f);

        if (GameManager.State == GameState.Waiting)
        {
            lastState = GameState.Waiting;

            Transform.position = waitingPosition;
            Transform.eulerAngles = waitingRotation;
            blur.interpolation = 1f;
        }
        else
        {
            //cache the starting lerp values
            //when the game goes to starting state
            if (lastState == GameState.Waiting)
            {
                lastState = GameState.Starting;


            }

            float t = startingPlayingCurve.Evaluate(GameManager.StartTime / GameManager.StartDuration);
            Transform.position = Vector3.Lerp(waitingPosition, playingPosition, t);

            Vector3 euler = Transform.eulerAngles;
            euler.x = Mathf.LerpAngle(waitingRotation.x, playingRotation.x, t);
            euler.y = Mathf.LerpAngle(waitingRotation.y, playingRotation.y, t);
            euler.z = Mathf.LerpAngle(waitingRotation.z, playingRotation.z, t);
            Transform.eulerAngles = euler;

            blur.interpolation = Mathf.Lerp(blur.interpolation, GameManager.Paused ? 1f : 0f, t);

            Transform.position += UnityEngine.Random.onUnitSphere * shake;
        }

        shake = Mathf.Lerp(shake, 0f, Time.deltaTime * shakeSettle);
        blur.enabled = blur.interpolation > 0.01f;

        if (GameManager.State == GameState.Starting)
        {
            startingTime += Time.fixedDeltaTime;
        }
        else
        {
            startingTime = 0f;
        }

        //if celeberating, focus on the winner
        if (GameManager.State == GameState.Celebrating && GameManager.Winner)
        {
            celebratingTime += Time.fixedDeltaTime;
            float t = celebratingTime / GameManager.CelebrateDuration;
            cam.fieldOfView = Mathf.Lerp(fov, 5f, focusOnWinner.Evaluate(t));
            Transform.LookAt(GameManager.Winner.transform.position);
        }
        else
        {
            celebratingTime = 0f;
        }
    }

    public static void Shake()
    {
        if (!instance) instance = FindObjectOfType<CameraManager>();

        instance.shake = 1f;
    }
}
