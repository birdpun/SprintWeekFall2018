using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blur = SuperBlur.SuperBlur;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    public float shakeSettle = 5f;

    private Blur blur;
    private float shake;

    private void Awake()
    {
        blur = GetComponent<Blur>();
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        Vector3 waitingPosition = new Vector3(0f, 60f, -140f);
        Vector3 playingPosition = new Vector3(0f, 60f, -70f);

        if (GameManager.State == GameState.Waiting)
        {
            transform.position = Vector3.Lerp(transform.position, waitingPosition, Time.fixedDeltaTime * 3f);
            transform.eulerAngles = new Vector3(21f, 0f, 0f);
            blur.interpolation = 1f;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, playingPosition, Time.fixedDeltaTime * 3f);

            Vector3 euler = transform.eulerAngles;
            euler.x = Mathf.LerpAngle(euler.x, 40f, Time.fixedDeltaTime * 3f);
            transform.eulerAngles = euler;

            blur.interpolation = Mathf.Lerp(blur.interpolation, GameManager.Paused ? 1f : 0f, Time.fixedDeltaTime * 3f);

            transform.position += UnityEngine.Random.onUnitSphere * shake;
        }

        shake = Mathf.Lerp(shake, 0f, Time.deltaTime * shakeSettle);
        blur.enabled = blur.interpolation > 0.01f;
    }

    public static void Shake()
    {
        if (!instance) instance = FindObjectOfType<CameraManager>();

        instance.shake = 1f;
    }
}
