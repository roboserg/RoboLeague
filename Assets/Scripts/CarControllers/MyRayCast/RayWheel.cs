using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RayWheel : MonoBehaviour
{
    Rigidbody _rb;
    RayController _carController;

    public float springStiffness;
    public float damperStiffness;
    public float restLength;
    public float springTravel;
    public float wheelRadius;

    public bool wheelFL, wheelFR, wheelBL, wheelBR;

    public float steerAngle;
    public GameObject wheelMesh;

    float _minLength, _maxLength;
    float _springLength, _springForce, _damperForce;
    float _lastLength, _springVelocity;
    Vector3 _suspensionForceVector, _wheelVelocityLs;
    private float Fx, Fy;
    public bool isWheelSurface = false;


    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _carController = GetComponentInParent<RayController>();
        _minLength = restLength - springTravel;
        _maxLength = restLength + springTravel;
        _springLength = _maxLength;
    }

    void FixedUpdate()
    {
        isWheelSurface = false;

        _minLength = restLength - springTravel;
        _maxLength = restLength + springTravel;
        _springLength = _maxLength;

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, _maxLength + wheelRadius))
        {
            isWheelSurface = true;
            _springLength = hit.distance - wheelRadius;
            _springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);
            _springForce  = springStiffness * (restLength - _springLength);

            _springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime;
            _damperForce    = damperStiffness * _springVelocity;

            _suspensionForceVector = (_springForce + _damperForce) * transform.up;

            if (!_carController.isJumpingRightNow)
                _rb.AddForceAtPosition(_suspensionForceVector, hit.point);

            float drift = 2;
            if (Input.GetKey(KeyCode.LeftShift))
                drift = 16;
            
            
            _wheelVelocityLs = transform.InverseTransformDirection(_rb.GetPointVelocity(hit.point));
            Fy = _wheelVelocityLs.x * _carController.Torque / drift;
            

            if (Input.GetAxis("Vertical") > 0 && _carController.forwardSpeed < 0.1f)
            {
                Fx = Input.GetAxis("Vertical") * 3500/100;
            }
            else if (Input.GetAxis("Vertical") < 0 && _carController.forwardSpeed > 0.1f)
            {
                Fx = Input.GetAxis("Vertical") * 3500 / 100;
            }
            else
                Fx = Input.GetAxis("Vertical") * GetThrottleSpeed() / 4;

            //Debug.Log(Fx);
            //rb.AddForceAtPosition(Fx * transform.forward, rb.transform.position);
            //if(carController.forwardSpeed < carController.maxSpeedNoBoost)
            _rb.AddForceAtPosition(Fx * transform.forward, _carController.transform.position, ForceMode.Acceleration);
            
            _rb.AddForceAtPosition(Fy * -transform.right, hit.point);
            
            _lastLength = _springLength;
            Debug.DrawRay(transform.position, transform.forward);
        }
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.up * steerAngle);
        if (wheelMesh)
        {
            wheelMesh.transform.position = transform.position - transform.up * _springLength;
            wheelMesh.transform.localRotation = transform.localRotation;
        }

        Debug.DrawRay(transform.position, -transform.up * _springLength, Color.black);
    }

    float GetThrottleSpeed()
    {
        float throttle = 0;
        float forwardSpeed = Mathf.Abs(_carController.forwardSpeed);

        if (forwardSpeed > (1410 / 100))
            throttle = 0;
        else if (forwardSpeed > (1400 / 100))
            throttle = _carController.scale(14, 14.1f, 1.6f, 0, forwardSpeed);
        else if (forwardSpeed <= (1400 / 100))
            throttle = _carController.scale(0, 14, 16, 1.6f, forwardSpeed);

        return throttle;
    }

    private void OnDrawGizmos()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit myHit, _maxLength + wheelRadius))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * (_springLength));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, myHit.point);
            Gizmos.DrawSphere(myHit.point, 0.03f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * (_springLength));
        }

        //Handles.color = Color.green;
        //Handles.DrawWireDisc(transform.position - transform.up * (springLength), transform.right, wheelRadius);
        //Handles.DrawWireArc(transform.position - transform.up * (springLength), transform.forward, transform.up, 360, wheelRadius);
    }
}