using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlaying : MonoBehaviour
{
    public Text textBlue;
    public Text textRed;

    private void Update()
    {
        textBlue.text = GameManager.Blue.lives.ToString();
        textRed.text = GameManager.Red.lives.ToString();
    }
}
