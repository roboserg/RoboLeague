using UnityEngine;

[RequireComponent(typeof(CubeController))]
public class CubeGroundControl : MonoBehaviour
{
    [Header("Steering")]
    public float turnRadiusCoefficient = 50;
    public float currentSteerAngle;
    
    [Header("Drift")]
    public float driftTime = 3;
    public float currentWheelSideFriction = 10;
    public float wheelSideFriction = 8;
    public float wheelSideFrictionDrift = 0.5f;
    
    Rigidbody _rb;
    CubeController _c;
    CubeWheel[] _wheelArray;
    
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _c = GetComponent<CubeController>();
        _wheelArray = GetComponentsInChildren<CubeWheel>();
    }

    private void Update()
    {
        // Sliding / drifting, lowers the wheel side friction when drifting
        var currentDrift = GameManager.InputManager.isDrift ? wheelSideFrictionDrift : wheelSideFriction;
        currentWheelSideFriction = Mathf.MoveTowards(currentWheelSideFriction, currentDrift, Time.deltaTime * driftTime);
    }
    
    private void FixedUpdate()
    {
        var throttleInput = GameManager.InputManager.throttleInput;
        
        // Throttle
        float forwardAcceleration = 0;

        if (GameManager.InputManager.isBoost)
            forwardAcceleration = GetForwardAcceleration(_c.forwardSpeedAbs);
        else
            forwardAcceleration = throttleInput * GetForwardAcceleration(_c.forwardSpeedAbs);
        
        // Braking
        if(_c.forwardSpeedSign != Mathf.Sign(throttleInput) && throttleInput != 0)
            forwardAcceleration += -1 * _c.forwardSpeedSign * 35;
        
        // Steering
        currentSteerAngle = CalculateSteerAngle();

        // Apply forces and steer angle to each wheel
        foreach (var wheel in _wheelArray)
        {
            //TODO: Func. call like this below OR Wheel class fetches data from this class?
            // Also probably should be an interface to a concrete implementation. Same for the NaiveGroundControl below.
            if (_c.isCanDrive) 
                wheel.ApplyForwardForce(forwardAcceleration / 4);
            
            if (wheel.wheelFL || wheel.wheelFR) 
                wheel.RotateWheels(currentSteerAngle);
        }
    }

    private float CalculateForwardForce(float input, float speed)
    {
        return  input * GetForwardAcceleration(_c.forwardSpeedAbs);
    }
    
    private float CalculateSteerAngle()
    {
        var curvature = 1 / GetTurnRadius(_c.forwardSpeed);
        return GameManager.InputManager.steerInput *  curvature * turnRadiusCoefficient;
    }
    
    static float GetForwardAcceleration(float speed)
    {
        // Replicates acceleration curve from RL, depends on current car forward velocity
        speed = Mathf.Abs(speed);
        float throttle = 0;
        
        if (speed > (1410 / 100))
            throttle = 0;
        else if (speed > (1400 / 100))
            throttle = CubeController.Scale(14, 14.1f, 1.6f, 0, speed);
        else if (speed <= (1400 / 100))
            throttle = CubeController.Scale(0, 14, 16, 1.6f, speed);

        return throttle;
    }

    static float GetTurnRadius(float speed)
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
    
    float _naiveRotationForce = 5;
    float _naiveRotationDampeningForce = -10;
    private void NaiveGroundControl()
    {
        if (_c.carState != CubeController.CarStates.AllWheelsSurface &&
            _c.carState != CubeController.CarStates.AllWheelsGround) return;

        // Throttle
        var throttleInput = Input.GetAxis("Vertical");
        float Fx = throttleInput * GetForwardAcceleration(_c.forwardSpeedAbs);
        _rb.AddForceAtPosition(Fx * transform.forward, _rb.transform.TransformPoint(_rb.centerOfMass),
            ForceMode.Acceleration);

        // Auto dampening
        _rb.AddForce(transform.forward * (5.25f * -Mathf.Sign(_c.forwardSpeed) * (1 - Mathf.Abs(throttleInput))),
            ForceMode.Acceleration);
        // alternative auto dampening
        //if (throttleInput == 0) _rb.AddForce(transform.forward * (5.25f * -Mathf.Sign(forwardSpeed)), ForceMode.Acceleration); 

        // Steering
        _rb.AddTorque(transform.up * (Input.GetAxis("Horizontal") * _naiveRotationForce), ForceMode.Acceleration);
        _rb.AddTorque(transform.up * (_naiveRotationDampeningForce * (1 - Mathf.Abs(Input.GetAxis("Horizontal"))) *
                                      transform.InverseTransformDirection(_rb.angularVelocity).y), ForceMode.Acceleration);
    }
}