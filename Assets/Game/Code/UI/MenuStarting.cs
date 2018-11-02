using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuStarting : MonoBehaviour
{
    public Text text;

    private string lastText;

    private void Update()
    {
        //if the text changed, increase its size
        if (text.text != lastText)
        {
            lastText = text.text;
            text.rectTransform.localScale = Vector3.one * 1.5f;
            FMODUnity.RuntimeManager.CreateInstance("event:/Countdown1").start();
        }

        //slowly settle to 1, 1, 1
        text.rectTransform.localScale = Vector3.Lerp(text.rectTransform.localScale, Vector3.one, Time.deltaTime * 4f);

        float timeLeft = GameManager.StartDuration - GameManager.StartTime;
        text.text = Mathf.CeilToInt(timeLeft).ToString();
    }
}
