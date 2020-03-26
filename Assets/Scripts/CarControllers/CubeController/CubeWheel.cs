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
    CubeWheelForces _wheelForces;
    
    private float wheelRadius;
    private Vector3 wheelVelocity;
    private float wheelForwardVelocity;
    private float wheelLateralVelocity;
    private Vector3 wheelContactPoint;
    private Vector3 lateralForcePosition;
    //private Vector3 wheelVelocityLocal;
    
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponentInParent<CubeController>();
        _wheelForces= GetComponentInParent<CubeWheelForces>();
    }

    private float angle;
    void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.up * steerAngle);
        if (wheelMesh)
        {
            wheelMesh.transform.position = transform.position;
            wheelMesh.transform.localRotation = transform.localRotation;
            angle += (Time.deltaTime * transform.InverseTransformDirection(wheelVelocity).z) / (2 * Mathf.PI * wheelRadius) * 360;
            //transform.Rotate(new Vector3(0, 1, 0), wheel.steerAngle - transform.localEulerAngles.y);
            wheelMesh.transform.Rotate(Vector3.right, angle);
        }
    }
    
    private void FixedUpdate()
    {
        if (_controller.carState != CubeController.CarStates.AllWheelsSurface &&
            _controller.carState != CubeController.CarStates.AllWheelsGround) return;
        
        // Math
        wheelRadius = transform.localScale.z / 2;
        wheelContactPoint = transform.position - transform.up * wheelRadius;
        wheelVelocity = _rb.GetPointVelocity(wheelContactPoint);
        wheelForwardVelocity = Vector3.Dot(wheelVelocity, transform.forward);
        wheelLateralVelocity = Vector3.Dot(wheelVelocity, transform.right);
        
        // Fx Forward Force
        _rb.AddForce(forwardForce * transform.forward, ForceMode.Acceleration);
        _rb.AddForce(transform.forward * (5.25f / 4 * -Mathf.Sign(_controller.forwardSpeed) * (1 - Mathf.Abs(_wheelForces._throttleInput))), ForceMode.Acceleration);
        
        // Fy Lateral Force
        Fy = wheelLateralVelocity * _wheelForces.currentWheelSideFriction * transform.right;
        lateralForcePosition = new Vector3(transform.position.x, _controller.cogLow.transform.position.y, transform.position.z);
        _rb.AddForceAtPosition(-Fy, lateralForcePosition, ForceMode.Acceleration);
    }

    private void OnDrawGizmos()
    {   
        // wheelContactPoint
        Gizmos.DrawSphere(wheelContactPoint, 0.03f);
        
        // Draw fake wheel
        Handles.color = Color.green;

        if (_rb == null) return;
        if (_controller.carState != CubeController.CarStates.AllWheelsSurface &&
            _controller.carState != CubeController.CarStates.AllWheelsGround) return;
        
        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, transform.right, transform.up, 360, wheelRadius);
        
        // Draw Fx and Fy wheel velocities
        DrawRay(wheelContactPoint, 0.1f * wheelVelocity, Color.black);
        DrawRay(wheelContactPoint, transform.forward * (wheelForwardVelocity * 0.1f), Color.blue);
        DrawRay(wheelContactPoint, transform.right * (wheelLateralVelocity * 0.1f), Color.red);
        
        // Draw lateral friction Fy
        DrawRay(lateralForcePosition, 0.3f * -Fy, Color.magenta);
    }

    void DrawRay(Vector3 from, Vector3 direction, Color c )
    {
        Gizmos.color = c;
        Gizmos.DrawRay(from, direction);
        Gizmos.DrawSphere(from + direction, 0.03f);
    }
}

// Паша25 implementation for Fy
//var lateralVel = Vector3.Dot(_rb.GetPointVelocity(wheelContactPoint), transform.right);
//var sideForce = lateralVel / _wheelForces.sideFriction * transform.right;
//_rb.AddForceAtPosition(-sideForce, transform.position, ForceMode.Acceleration);