using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool isBoost = false;
    void Update()
    {
        isBoost = Input.GetButton("RB") || Input.GetMouseButton(0);
    }

    public string axisName = "Horizontal";
    public AnimationCurve sensitivityCurve;
    private float _vel = 0;
    float _currentHorizontalInput = 0;
    public float GetValue()
    {
        var target = Mathf.MoveTowards(_currentHorizontalInput, Input.GetAxis(axisName), Time.fixedDeltaTime / 25);
        _currentHorizontalInput = sensitivityCurve.Evaluate(Mathf.Abs(target));
        var ret = _currentHorizontalInput * Mathf.Sign(Input.GetAxis(axisName));
        
        // Debug.Log("Input: " + Input.GetAxis("Horizontal"));
        // Debug.Log("Out: " + ret);
        // Debug.Log("");

        return ret;

        //var ret = sensitivityCurve.Evaluate(Mathf.Abs(Input.GetAxis(axisName)));
        //return ret * Mathf.Sign(ret);

        //var target = sensitivityCurve.Evaluate(Mathf.Abs(Input.GetAxis(axisName)));
        //currentInput = Mathf.SmoothDamp(currentInput, target, ref vel, 1f, Mathf.Infinity, Time.fixedDeltaTime);
        //return currentInput * Mathf.Sign(Input.GetAxis(axisName));
    }
    
    private void OnGUI()
    {
        GUILayout.HorizontalSlider(Input.GetAxis(axisName), -1, 1, GUILayout.Width(200));
        GUILayout.HorizontalSlider(GetValue(), -1, 1, GUILayout.Width(200));
    }
}
