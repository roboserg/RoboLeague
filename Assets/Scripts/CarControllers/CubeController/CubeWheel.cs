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
    
    //[HideInInspector]
    public bool isDrawWheelVelocities, isDrawWheelDisc, isDrawForces;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _c = GetComponentInParent<CubeController>();
        _groundControl= GetComponentInParent<CubeGroundControl>();
        _wheelRadius = transform.localScale.z / 2;
    }
    
    public void RotateWheels(float steerAngle)
    {
        if(wheelFL || wheelFR)
            transform.localRotation = Quaternion.Euler(Vector3.up * steerAngle);
        
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
        
        //TODO Make a separate function
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

    #region DrawDebugGizmos

    private void OnDrawGizmos()
    {
        //_wheelRadius = transform.localScale.z / 2;

        // DrawWheelDisc();
        // DrawWheelContactPoint();
        //
        // if (isDrawWheelVelocities)
        //     DrawWheelVelocities();
        //
        // if(isDrawForces)
        //     DrawForces();
    }

    private void DrawWheelDisc()
    {
#if UNITY_EDITOR
        if (isDrawWheelDisc)
        {
            Handles.color = Color.black;
            if (_rb != null)
                Handles.color = _c.isCanDrive ? Color.green : Color.red;

            Handles.DrawWireArc(transform.position, transform.right, transform.up, 360, _wheelRadius);
        }
#endif
    }

    private void DrawWheelContactPoint()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position - transform.up * _wheelRadius, 0.02f);
    }

    private void DrawWheelVelocities()
    {
        if (_rb == null) return;
        if (_c.isCanDrive != true) return;   
        
        var offset = 0.05f * transform.up;
        RoboUtils.DrawRay(_wheelContactPoint + offset, _wheelVelocity * 0.1f, Color.black);
        RoboUtils.DrawRay(_wheelContactPoint + offset, (_wheelForwardVelocity * 0.1f) * transform.forward, Color.blue);
        RoboUtils.DrawRay(_wheelContactPoint + offset, (_wheelLateralVelocity * 0.1f) * transform.right, Color.red);
    }

    private void DrawForces()
    {
        if (_c.isCanDrive != true) return;   
        
        // Draw induced lateral friction Fy
        RoboUtils.DrawRay(_lateralForcePosition, 0.3f * -Fy * transform.right, Color.magenta);

        // Draw observed forces
        RoboUtils.DrawLocalRay(transform, transform.up, _wheelAcceleration.z, transform.forward, Color.gray);
    }
    
    #endregion
}