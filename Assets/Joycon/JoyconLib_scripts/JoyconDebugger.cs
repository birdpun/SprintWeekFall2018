using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconDebugger : MonoBehaviour
{
    private Material material;
    private Mesh mesh;

    private void Awake()
    {
        material = new Material(Shader.Find("Standard"));
        mesh = CreateCube();
    }

    private Mesh CreateCube()
    {
        Vector3[] vertices =
        {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= Vector3.one * 0.5f;
        }

        int[] triangles =
        {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnGUI()
    {
        for (int i = 0; i < JoyconManager.Joycons.Count; i++)
        {
            Joycon joycon = JoyconManager.Joycons[i];

            string name = "Joycon " + i;
            name += " " + joycon.SerialNumber;
            GUILayout.Label(name);
            GUILayout.Label("\tType: " + joycon.Type);
            if (joycon.isLeft)
            {
                GUILayout.Label("\tLeft: " + joycon.DPadLeft);
                GUILayout.Label("\tRight: " + joycon.DPadRight);
                GUILayout.Label("\tUp: " + joycon.DPadUp);
                GUILayout.Label("\tDown: " + joycon.DPadDown);
            }
            else
            {
                GUILayout.Label("\tY: " + joycon.Y);
                GUILayout.Label("\tA: " + joycon.A);
                GUILayout.Label("\tX: " + joycon.X);
                GUILayout.Label("\tB: " + joycon.B);
            }
            GUILayout.Label("\tAccel: " + joycon.Acceleration);
            GUILayout.Label("\tGyro: " + joycon.Gyro);
            GUILayout.Label("\tVector: " + joycon.Rotation.eulerAngles);

            float a = Vector3.Angle(Vector3.forward, joycon.Rotation * Vector3.forward);
            a -= 90f;

            GUILayout.Label("\tAngle: " + joycon.Euler.z);
        }
    }

    private void Update()
    {
        float size = 1.25f;
        Vector3 start = Vector3.left * JoyconManager.Joycons.Count * -0.5f * size;
        Vector3 end = Vector3.left * JoyconManager.Joycons.Count * 0.5f * size;

        for (int i = 0; i < JoyconManager.Joycons.Count; i++)
        {
            Joycon joycon = JoyconManager.Joycons[i];
            float t = i / (float)(JoyconManager.Joycons.Count - 1);
            Vector3 position = Vector3.Lerp(start, end, t);

            Graphics.DrawMesh(mesh, position, joycon.Rotation, material, 0);

            Debug.DrawRay(position, joycon.Right, Color.red);
            Debug.DrawRay(position, joycon.Up, Color.green);
            Debug.DrawRay(position, joycon.Forward, Color.blue);
        }
    }
}