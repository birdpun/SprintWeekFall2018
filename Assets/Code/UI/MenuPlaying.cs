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

    private void Update()
    {
        //if the text changed, increase its size
        if (textBlue.text != lastTextBlue)
        {
            lastTextBlue = textBlue.text;
            textBlue.rectTransform.localScale = Vector3.one * 1.5f;
        }
        if (textRed.text != lastTextRed)
        {
            lastTextRed = textRed.text;
            textRed.rectTransform.localScale = Vector3.one * 1.5f;
        }

        //slowly settle to 1, 1, 1
        textBlue.rectTransform.localScale = Vector3.Lerp(textBlue.rectTransform.localScale, Vector3.one, Time.deltaTime * 9f);
        textRed.rectTransform.localScale = Vector3.Lerp(textRed.rectTransform.localScale, Vector3.one, Time.deltaTime * 9f);

        textBlue.text = GameManager.Blue.lives.ToString();
        textRed.text = GameManager.Red.lives.ToString();
    }
}
