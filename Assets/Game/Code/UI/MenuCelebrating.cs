using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCelebrating : MonoBehaviour
{
    public Text text;

    private void Update()
    {
        if (GameManager.Winner)
        {
            text.text = GameManager.Winner.name+" wins!";

        }
        else
        {
            text.text = "";
        }
    }
}
