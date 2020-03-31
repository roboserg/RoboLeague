using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CubeWheel : MonoBehaviour
{
  //  public float steerAngle;
  //  public float Fx;
    float Fy;
    
    public bool wheelFL, wheelFR, wheelRL, wheelRR;
    
    public Transform wheelMesh;
    private float _meshRevolutionAngle;
    
    Rigidbody _rb;
    CubeController _c;
    CubeGroundControl _groundControl;
    
    float _wheelRadius, _wheelForwardVelocity, _wheelLateralVelocity;
    Vector3 _wheelVelocity, _lastWheelVelocity, _wheelAcceleration, _wheelContactPoint, _lateralForcePosition = Vector3.zero;
    
    const float AutoBrakeAcceleration = 5.25f;
    
    // Debug Draw
    [Title("Debug Draw Options")]
    [Button("@\"Draw Wheel Velocities: \" + _isDrawWheelVelocities", ButtonSizes.Large)]
    void IsDrawWheelVelocities()
    {
        isDrawWheelVelocities = !isDrawWheelVelocities;
    }
    [HideInInspector]
    public bool isDrawWheelVelocities, isDrawWheelDisc, isDrawForces;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _c = GetComponentInParent<CubeController>();
        _groundControl= GetComponentInParent<CubeGroundControl>();
        _wheelRadius = transform.localScale.z / 2;
    }

    
    void Update()
    {
        // Update mesh rotations of the wheel
        if (wheelMesh)
        {
            //wheelMesh.transform.position = transform.position;
            wheelMesh.transform.localRotation = transform.localRotation;
            _meshRevolutionAngle += (Time.deltaTime * transform.InverseTransformDirection(_wheelVelocity).z) /
                (2 * Mathf.PI * _wheelRadius) * 360;
            wheelMesh.transform.Rotate(Vector3.right, _meshRevolutionAngle * 1.3f);
            //transform.Rotate(new Vector3(0, 1, 0), steerAngle - transform.localEulerAngles.y);
        }
    }

    public void RotateWheels(float steerAngle)
    {
        transform.localRotation = Quaternion.Euler(Vector3.up * steerAngle);
    }

    private void FixedUpdate()
    {
        UpdateWheelState();

        if (!_c.isCanDrive) return;
        //ApplyForwardForce();
        ApplyLateralForce();
        SimulateDrag();
    }
    
    public void ApplyForwardForce(float force)
    {
        _rb.AddForce(force * transform.forward, ForceMode.Acceleration);
        
        // Kill velocity to 0 for small car velocities
        if (force == 0 && _c.forwardSpeedAbs < 0.1)
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, 0);
    }
    
    private void ApplyLateralForce()
    {
        Fy = _wheelLateralVelocity * _groundControl.currentWheelSideFriction ;
        _lateralForcePosition = transform.localPosition;
        _lateralForcePosition.y = _c.cogLow.localPosition.y;
        _lateralForcePosition = _c.transform.TransformPoint(_lateralForcePosition);
        _rb.AddForceAtPosition(-Fy * transform.right, _lateralForcePosition, ForceMode.Acceleration);
    }
    
    private void SimulateDrag()
    {
        //Applies auto braking if no input, simulates air and ground drag
        if (!(_c.forwardSpeedAbs >= 0.1)) return;
        
        //TODO Use constants, also make a separate function
        var dragForce = AutoBrakeAcceleration / 4 * _c.forwardSpeedSign * (1 - Mathf.Abs(GameManager.InputManager.throttleInput));
        _rb.AddForce(-dragForce * transform.forward, ForceMode.Acceleration);
    }

    private void UpdateWheelState()
    {
        _wheelContactPoint = transform.position - transform.up * _wheelRadius;
        _wheelVelocity = _rb.GetPointVelocity(_wheelContactPoint);
        _wheelForwardVelocity = Vector3.Dot(_wheelVelocity, transform.forward);
        _wheelLateralVelocity = Vector3.Dot(_wheelVelocity, transform.right);

        _wheelAcceleration = (_wheelVelocity - _lastWheelVelocity) * Time.fixedTime;
        _lastWheelVelocity = _wheelVelocity;
    }

    private void OnDrawGizmos()
    {
        _wheelRadius = transform.localScale.z / 2;
        
        // Draw wheel disc
        if (isDrawWheelDisc)
        {
            Handles.color = Color.black;
            if (_rb != null)
                Handles.color = _c.isCanDrive ? Color.green : Color.red;

            Handles.DrawWireArc(transform.position, transform.right, transform.up, 360, _wheelRadius);
        }
        
        if (_rb == null) return;
        
        // wheelContactPoint
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position - transform.up * _wheelRadius, 0.02f);
        
        if (_c.isCanDrive != true) return;      
        
        // Draw observed Vx and Vy wheel velocities
        if (isDrawWheelVelocities)
        {
            DrawRay(_wheelContactPoint, _wheelVelocity * 0.1f, Color.black);
            DrawRay(_wheelContactPoint, (_wheelForwardVelocity * 0.1f) * transform.forward, Color.blue);
            DrawRay(_wheelContactPoint, (_wheelLateralVelocity * 0.1f) * transform.right, Color.red);    
        }

        if(isDrawForces)
        {
            // Draw induced lateral friction Fy
            DrawRay(_lateralForcePosition, 0.3f * -Fy * transform.right, Color.magenta);
            
            // Draw observed forces
            DrawLocalRay(transform.up, _wheelAcceleration.z, transform.forward, Color.gray);
        }
    }

    #region Unitls

    void DrawRay(Vector3 from, Vector3 direction, Color c )
    {
        Gizmos.color = c;
        Gizmos.DrawRay(from, direction);
        Gizmos.DrawSphere(from + direction, 0.02f);
    }
    
    void DrawLocalRay(Vector3 from, float length, Vector3 dir, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawRay(transform.position + from, length * dir);
        Gizmos.DrawSphere(transform.position + from + length * dir, 0.03f);
    }
    
    #endregion
}