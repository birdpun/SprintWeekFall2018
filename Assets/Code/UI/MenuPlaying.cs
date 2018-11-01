using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlaying : MonoBehaviour
{
    public Text textBlue;
    public Text textRed;

    private string lastTextBlue;
    private string lastTextRed;

    private float originalBlueX;
    private float originalRedX;

    private float blueShake;
    private float redShake;

    private void OnEnable()
    {
        originalBlueX = textBlue.rectTransform.anchoredPosition.x;
        originalRedX = textRed.rectTransform.anchoredPosition.x;
    }

    private void Update()
    {
        //if the text changed, increase its size
        if (textBlue.text != lastTextBlue)
        {
            lastTextBlue = textBlue.text;
            blueShake = 15f;
        }
        if (textRed.text != lastTextRed)
        {
            lastTextRed = textRed.text;
            redShake = 15f;
        }

        //slowly settle to 1, 1, 1
        float blueX = textBlue.rectTransform.anchoredPosition.x;
        float redX = textRed.rectTransform.anchoredPosition.x;

        blueX += Random.Range(-1f, 1f) * blueShake;
        redX += Random.Range(-1f, 1f) * redShake;

        blueShake = Mathf.Lerp(blueShake, 0f, Time.deltaTime * 10f);
        redShake = Mathf.Lerp(redShake, 0f, Time.deltaTime * 10f);

        blueX = Mathf.Lerp(blueX, originalBlueX, Time.deltaTime * 9f);
        redX = Mathf.Lerp(redX, originalRedX, Time.deltaTime * 9f);

        textBlue.rectTransform.anchoredPosition = new Vector2(blueX, textBlue.rectTransform.anchoredPosition.y);
        textRed.rectTransform.anchoredPosition = new Vector2(redX, textRed.rectTransform.anchoredPosition.y);

        textBlue.text = GameManager.Blue.lives.ToString();
        textRed.text = GameManager.Red.lives.ToString();
    }
}
