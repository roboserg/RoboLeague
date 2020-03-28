using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CubeGroundControl : MonoBehaviour
{
    [Header("Steering")]
    public float turnRadiusCoefficient = 40;
    public float currentSteerAngle;
    
    [Header("Drift")]
    public float driftTime;
    public float currentWheelSideFriction = 10;
    public float WheelSideFriction = 8;
    public float WheelSideFrictionDrift = 0.5f;
    
    CubeWheel[] _wheelArray;
    
    Rigidbody _rb;
    CubeController _controller;
    
    float naiveRotationForce = 5;
    float naiveRotationDampeningForce = -10;
    
    [HideInInspector] public float _throttleInput;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponent<CubeController>();
        _wheelArray = GetComponentsInChildren<CubeWheel>();
    }

    private void Update()
    {
        // Process Input 
        _throttleInput = 0;
        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("RT") > 0)
            _throttleInput = Mathf.Max(Input.GetAxis("Vertical"), Input.GetAxis("RT"));
        else if (Input.GetAxis("Vertical") < 0 || Input.GetAxis("LT") < 0)
            _throttleInput = Mathf.Min(Input.GetAxis("Vertical"), Input.GetAxis("LT"));

        if (Input.GetButton("LB") || Input.GetKey(KeyCode.LeftShift))
        {
            currentWheelSideFriction = Mathf.Lerp(currentWheelSideFriction, WheelSideFrictionDrift, Time.deltaTime * driftTime);
            //currentWheelSideFriction = WheelSideFrictionDrift;
        }
        else
        {
            currentWheelSideFriction = Mathf.Lerp(currentWheelSideFriction, WheelSideFriction, Time.deltaTime * driftTime);
            //currentWheelSideFriction = WheelSideFriction;
        }
    }

    
    private void FixedUpdate()
    {
        //if(_controller.isCanDrive)

        float Fx = _throttleInput * GetThrottleSpeed();
        currentSteerAngle = (1 / GetTurnRadius(_controller.forwardSpeed)) * turnRadiusCoefficient * Input.GetAxis("Horizontal");
        
        foreach (var w in _wheelArray)
        {
            if (_controller.isCanDrive) w.forwardForce = Fx / 4;
            
            if (w.wheelFL || w.wheelFR) 
                w.steerAngle = currentSteerAngle;
        }
    }

    float GetThrottleSpeed()
    {
        float throttle = 0;

        float myForwardSpeed = Mathf.Abs(_controller.forwardSpeed);

        if (myForwardSpeed > (1410 / 100))
            throttle = 0;
        else if (myForwardSpeed > (1400 / 100))
            throttle = CubeController.Scale(14, 14.1f, 1.6f, 0, myForwardSpeed);
        else if (myForwardSpeed <= (1400 / 100))
            throttle = CubeController.Scale(0, 14, 16, 1.6f, myForwardSpeed);

        return throttle;
    }
    
    float GetTurnRadius(float forwardSpeed)
    {
        float turnRadius = 0;
        var curvature = CubeController.Scale(0, 5, 0.0069f, 0.00398f, forwardSpeed);

        if (forwardSpeed >= 500 / 100)
            curvature = CubeController.Scale(5, 10, 0.00398f, 0.00235f, forwardSpeed);

        if (forwardSpeed >= 1000 / 100)
            curvature = CubeController.Scale(10, 15, 0.00235f, 0.001375f, forwardSpeed);

        if (forwardSpeed >= 1500 / 100)
            curvature = CubeController.Scale(15, 17.5f, 0.001375f, 0.0011f, forwardSpeed);

        if (forwardSpeed >= 1750 / 100)
            curvature = CubeController.Scale(17.5f, 23, 0.0011f, 0.00088f, forwardSpeed);

        turnRadius = 1 / (curvature * 100);
        return turnRadius;
    }
    
    private void NaiveGroundControl()
    {
        if (_controller.carState != CubeController.CarStates.AllWheelsSurface &&
            _controller.carState != CubeController.CarStates.AllWheelsGround) return;

        // Throttle
        var throttleInput = Input.GetAxis("Vertical");
        float Fx = throttleInput * GetThrottleSpeed();
        _rb.AddForceAtPosition(Fx * transform.forward, _rb.transform.TransformPoint(_rb.centerOfMass),
            ForceMode.Acceleration);

        // Auto dampening
        _rb.AddForce(transform.forward * (5.25f * -Mathf.Sign(_controller.forwardSpeed) * (1 - Mathf.Abs(throttleInput))),
            ForceMode.Acceleration);
        // alternative auto dampening
        //if (throttleInput == 0) _rb.AddForce(transform.forward * (5.25f * -Mathf.Sign(forwardSpeed)), ForceMode.Acceleration); 

        // Steering
        _rb.AddTorque(transform.up * (Input.GetAxis("Horizontal") * naiveRotationForce), ForceMode.Acceleration);
        _rb.AddTorque(transform.up * (naiveRotationDampeningForce * (1 - Mathf.Abs(Input.GetAxis("Horizontal"))) *
                                      transform.InverseTransformDirection(_rb.angularVelocity).y), ForceMode.Acceleration);
    }
}

internal static class MyClass
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        //Selection.activeGameObject = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<BoxCollider>().gameObject;
        //SceneView.FrameLastActiveSceneView();
    }
}