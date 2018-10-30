using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Vector3[] velocities = { };
    private Vector3[] lastVectors = { };

    private void OnGUI()
    {
        int index = 0;
        foreach (var joycon in JoyconManager.Joycons)
        {
            GUILayout.Label("joycon " + index.ToString());
            GUILayout.Label("\tIs Left: " + joycon.isLeft.ToString());
            if (joycon.isLeft)
            {
                GUILayout.Label("\tLeft: " + joycon.GetButton(Joycon.Button.DPAD_LEFT));
                GUILayout.Label("\tRight: " + joycon.GetButton(Joycon.Button.DPAD_RIGHT));
                GUILayout.Label("\tUp: " + joycon.GetButton(Joycon.Button.DPAD_UP));
                GUILayout.Label("\tDown: " + joycon.GetButton(Joycon.Button.DPAD_DOWN));
            }
            else
            {
                GUILayout.Label("\tY: " + joycon.GetButton(Joycon.Button.DPAD_LEFT));
                GUILayout.Label("\tA: " + joycon.GetButton(Joycon.Button.DPAD_RIGHT));
                GUILayout.Label("\tX: " + joycon.GetButton(Joycon.Button.DPAD_UP));
                GUILayout.Label("\tB: " + joycon.GetButton(Joycon.Button.DPAD_DOWN));
            }
            GUILayout.Label("\tAccel: " + joycon.GetAccel());
            GUILayout.Label("\tGyro: " + joycon.GetGyro());
            GUILayout.Label("\tVector: " + joycon.GetVector().eulerAngles);
            GUILayout.Label("\tVelocity: " + velocities[index].y);

            index++;
        }
    }

    private void Update()
    {
        if (lastVectors.Length != JoyconManager.Joycons.Count)
        {
            lastVectors = new Vector3[JoyconManager.Joycons.Count];
            velocities = new Vector3[lastVectors.Length];
        }

        for (int i = 0; i < JoyconManager.Joycons.Count; i++)
        {
            var joycon = JoyconManager.Joycons[i];
            velocities[i] = joycon.GetVector().eulerAngles - lastVectors[i];
            lastVectors[i] = joycon.GetVector().eulerAngles;
        }
    }
}
