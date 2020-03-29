using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CubeGroundControl : MonoBehaviour
{
    [Header("Steering")]
    public float steerSensivity;
    public float turnRadiusCoefficient = 40;
    public float currentSteerAngle;
    
    [Header("Drift")]
    public float driftTime;
    public float currentWheelSideFriction = 10;
    public float wheelSideFriction = 8;
    public float wheelSideFrictionDrift = 0.5f;
    
    CubeWheel[] _wheelArray;
    
    Rigidbody _rb;
    CubeController _controller;
    
    float naiveRotationForce = 5;
    float naiveRotationDampeningForce = -10;

    [HideInInspector] public float throttleInput = 0, steerInput = 0;
    
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponent<CubeController>();
        _wheelArray = GetComponentsInChildren<CubeWheel>();
    }

    private void Update()
    {
        //TODO: Move input processing to a dedicated input manager static class
        // Process Input 
        {
            throttleInput = 0;
            if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("RT") > 0)
                throttleInput = Mathf.Max(Input.GetAxis("Vertical"), Input.GetAxis("RT"));
            else if (Input.GetAxis("Vertical") < 0 || Input.GetAxis("LT") < 0)
                throttleInput = Mathf.Min(Input.GetAxis("Vertical"), Input.GetAxis("LT"));

            // Sliding / drifting, lowers the wheel side friction when drifting
            if (Input.GetButton("LB") || Input.GetKey(KeyCode.LeftShift))
                currentWheelSideFriction = Mathf.Lerp(currentWheelSideFriction, wheelSideFrictionDrift,
                    Time.deltaTime * driftTime);
            else
                currentWheelSideFriction =
                    Mathf.Lerp(currentWheelSideFriction, wheelSideFriction, Time.deltaTime * driftTime);
        }
    }
    
    private void FixedUpdate()
    {
        var Fx = CalculateForwardForce();
        
        currentSteerAngle = CalculateSteerAngle();

        // Apply forces and steer angle to each wheel
        foreach (var w in _wheelArray)
        {
            if (_controller.isCanDrive) w.forwardForce = Fx / 4;
            
            if (w.wheelFL || w.wheelFR) 
                w.steerAngle = currentSteerAngle;
        }
    }

    private float CalculateSteerAngle()
    {
        steerInput = Mathf.MoveTowards(steerInput, Input.GetAxis("Horizontal"), Time.fixedDeltaTime * steerSensivity);
        return (1 / GetTurnRadius(_controller.forwardSpeed)) * turnRadiusCoefficient * steerInput;
    }

    private float CalculateForwardForce()
    {
        if (GameManager.InputManager.isBoost)
            throttleInput = 1;
        
        float Fx = throttleInput * GetThrottleSpeed();
        
        return Fx;
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
    
    float GetTurnRadius(float speed)
    {
        float forwardSpeed = Mathf.Abs(speed);
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

