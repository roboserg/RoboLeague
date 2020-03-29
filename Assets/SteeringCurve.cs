using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringCurve : MonoBehaviour
{
    public string axisName;
    public AnimationCurve sensitivityCurve;

    public float GetValue()
    {
        return sensitivityCurve.Evaluate(Input.GetAxis(axisName));
    }

    private void OnGUI()
    {
        GUILayout.HorizontalSlider(Input.GetAxis(axisName), -1, 1, GUILayout.Width(200));
        GUILayout.HorizontalSlider(GetValue(), -1, 1, GUILayout.Width(200));
    }
}
