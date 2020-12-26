using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float throttleInput, steerInput, yawInput, pitchInput, rollInput;
    public bool isBoost, isDrift, isAirRoll;
    public bool isJump, isJumpUp, isJumpDown;
    /*void Update()
    {
        throttleInput = GetThrottle();
        steerInput = GetSteerInput();
        
        yawInput = Input.GetAxis("Horizontal");
        pitchInput = Input.GetAxis("PitchAxis");
        rollInput = GetRollInput();

        isJump = Input.GetMouseButton(1) || Input.GetButton("A");
        isJumpUp = Input.GetMouseButtonUp(1) || Input.GetButtonUp("A");
        isJumpDown = Input.GetMouseButtonDown(1) || Input.GetButtonDown("A");

        isBoost = Input.GetButton("RB") || Input.GetMouseButton(0);
        isDrift = Input.GetButton("LB") || Input.GetKey(KeyCode.LeftShift);
        isAirRoll = Input.GetButton("LB") || Input.GetKey(KeyCode.LeftShift);
    }*/

    private static float GetRollInput()
    {
        var inputRoll = 0;
        if (Input.GetKey(KeyCode.E) || Input.GetButton("B"))
            inputRoll = -1;
        else if (Input.GetKey(KeyCode.Q) || Input.GetButton("Y"))
            inputRoll = 1;

        return inputRoll;
    }

    static float GetThrottle()
    {
        float throttle = 0;
        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("RT") > 0)
            throttle = Mathf.Max(Input.GetAxis("Vertical"), Input.GetAxis("RT"));
        else if (Input.GetAxis("Vertical") < 0 || Input.GetAxis("LT") < 0)
            throttle = Mathf.Min(Input.GetAxis("Vertical"), Input.GetAxis("LT"));

        return throttle;
    }

    static float GetSteerInput()
    {
        //return Mathf.MoveTowards(steerInput, Input.GetAxis("Horizontal"), Time.fixedDeltaTime);
        return Input.GetAxis("Horizontal");
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
        GUILayout.HorizontalSlider(pitchInput, -1, 1, GUILayout.Width(200));
        GUILayout.HorizontalSlider(yawInput, -1, 1, GUILayout.Width(200));
        GUILayout.HorizontalSlider(rollInput, -1, 1, GUILayout.Width(200));
    }
}
