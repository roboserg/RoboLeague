using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CubeWheel : MonoBehaviour
{
    public float steerAngle;
    public float forwardForce;

    public Vector3 Fy;
    
    public bool wheelFL, wheelFR, wheelRL, wheelRR;

    public Transform wheelMesh;
    
    Rigidbody _rb;
    CubeController _controller;
    CubeGroundControl _groundControl;
    
    float _wheelRadius;
    Vector3 _wheelVelocity = Vector3.zero;
    Vector3 _lastWheelVelocity = Vector3.zero;
    Vector3 _wheelAcceleration = Vector3.zero;
    float _wheelForwardVelocity;
    float _wheelLateralVelocity;
    Vector3 _wheelContactPoint;
    Vector3 _lateralForcePosition;
    //private Vector3 wheelVelocityLocal;

    [Title("Debug Draw Options")]
    [Button("@\"Draw Wheel Velocities: \" + _isDrawWheelVelocities", ButtonSizes.Large)]
    void IsDrawWheelVelocities()
    {
        isDrawWheelVelocities = !isDrawWheelVelocities;
    }
    [HideInInspector]
    public bool isDrawWheelVelocities;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponentInParent<CubeController>();
        _groundControl= GetComponentInParent<CubeGroundControl>();
    }

    private float _angle;
    void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.up * steerAngle);
        
        // Update mesh rotations of the wheel
        if (wheelMesh)
        {
            //wheelMesh.transform.position = transform.position;
            wheelMesh.transform.localRotation = transform.localRotation;
            _angle += (Time.deltaTime * transform.InverseTransformDirection(_wheelVelocity).z) / (2 * Mathf.PI * _wheelRadius) * 360;
            //transform.Rotate(new Vector3(0, 1, 0), wheel.steerAngle - transform.localEulerAngles.y);
            wheelMesh.transform.Rotate(Vector3.right, _angle * 1.3f);
        }
    }
    
    private void FixedUpdate()
    {
        // Math
        _wheelRadius = transform.localScale.z / 2;
        _wheelContactPoint = transform.position - transform.up * _wheelRadius;
        
        _wheelVelocity = _rb.GetPointVelocity(_wheelContactPoint);
        _wheelForwardVelocity = Vector3.Dot(_wheelVelocity, transform.forward);
        _wheelLateralVelocity = Vector3.Dot(_wheelVelocity, transform.right);
        
        _wheelAcceleration = (_wheelVelocity - _lastWheelVelocity) * Time.fixedTime;
        _lastWheelVelocity = _wheelVelocity;
        
        if(!_controller.isCanDrive) return;
        
        // Fx Forward Force
        _rb.AddForce(forwardForce * transform.forward, ForceMode.Acceleration);
        
        if(_groundControl._throttleInput == 0 && Mathf.Abs(_controller.forwardSpeed) > 0.1)
            //Apply auto braking if no input, simulates drag
            _rb.AddForce(transform.forward * (5.25f / 4 * -Mathf.Sign(_controller.forwardSpeed) * (1 - Mathf.Abs(_groundControl._throttleInput))), ForceMode.Acceleration);
        else if (_groundControl._throttleInput == 0 && Mathf.Abs(_controller.forwardSpeed) <= 0.1)
            // Kill velocity to 0 for small car velocities
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, 0);
        
        // Fy Lateral Force
        Fy = _wheelLateralVelocity * _groundControl.currentWheelSideFriction * transform.right;
        _lateralForcePosition = new Vector3(transform.position.x, _controller.cogLow.transform.position.y, transform.position.z);
        _rb.AddForceAtPosition(-Fy, _lateralForcePosition, ForceMode.Acceleration);
    }

    private void OnDrawGizmos()
    {   
        if (_rb == null) return; 
        
        // wheelContactPoint
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_wheelContactPoint, 0.03f);

        
        if (_controller.carState != CubeController.CarStates.AllWheelsSurface &&
            _controller.carState != CubeController.CarStates.AllWheelsGround) return;      
        
        // Draw observed Vx and Vy wheel velocities
        if (isDrawWheelVelocities)
        {
            DrawRay(_wheelContactPoint, _wheelVelocity * 0.1f, Color.black);
            DrawRay(_wheelContactPoint, (_wheelForwardVelocity * 0.1f) * transform.forward, Color.blue);
            DrawRay(_wheelContactPoint, (_wheelLateralVelocity * 0.1f) * transform.right, Color.red);    
        }
        
        // Draw induced lateral friction Fy
        DrawRay(_lateralForcePosition, 0.3f * -Fy, Color.magenta);
        
        // Draw observed forces
        DrawLocalRay(transform.up, _wheelAcceleration.z, transform.forward, Color.yellow);
    }

    void DrawRay(Vector3 from, Vector3 direction, Color c )
    {
        Gizmos.color = c;
        Gizmos.DrawRay(from, direction);
        Gizmos.DrawSphere(from + direction, 0.03f);
    }
    
    void DrawLocalRay(Vector3 from, float length, Vector3 dir, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawRay(transform.position + from, length * dir);
        Gizmos.DrawSphere(transform.position + from + length * dir, 0.03f);
    }
}

// Паша25 implementation for Fy
//var lateralVel = Vector3.Dot(_rb.GetPointVelocity(wheelContactPoint), transform.right);
//var sideForce = lateralVel / _wheelForces.sideFriction * transform.right;
//_rb.AddForceAtPosition(-sideForce, transform.position, ForceMode.Acceleration);