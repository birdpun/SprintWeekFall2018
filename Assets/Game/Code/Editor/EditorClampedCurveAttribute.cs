using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ClampedCurveAttribute))]
public class EditorClampedCurveAttribute : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.CurveField(position, property, Color.yellow, new Rect(0, 0, 1, 1));
    }
}
