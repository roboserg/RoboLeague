using UnityEngine;

[RequireComponent(typeof(CubeController))]
public class CubeGroundControl : MonoBehaviour
{
    [Header("Steering")]
    [Range(0,100)]
    public float turnRadiusCoefficient = 50;
    public float currentSteerAngle;
    
    [Header("Drift")]
    public float driftTime = 3;
    public float currentWheelSideFriction = 10;
    public float wheelSideFriction = 8;
    public float wheelSideFrictionDrift = 0.5f;
    
    Rigidbody _rb;
    CubeController _controller;
    CubeWheel[] _wheelArray;
    
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponent<CubeController>();
        _wheelArray = GetComponentsInChildren<CubeWheel>();
    }

    private void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        SetDriftFriction();
        
        var forwardAcceleration = CalcForwardForce(GameManager.InputManager.throttleInput);
        ApplyWheelForwardForce(forwardAcceleration);
        
        currentSteerAngle = CalculateSteerAngle();
        ApplyWheelRotation(currentSteerAngle);
    }
    
    private void SetDriftFriction()
    {
        // Sliding / drifting, lowers the wheel side friction when drifting
        var currentDriftDrag = GameManager.InputManager.isDrift ? wheelSideFrictionDrift : wheelSideFriction;
        currentWheelSideFriction = Mathf.MoveTowards(currentWheelSideFriction, currentDriftDrag, Time.deltaTime * driftTime);
    }

    private void ApplyWheelForwardForce(float forwardAcceleration)
    {
        // Apply forces to each wheel
        foreach (var wheel in _wheelArray)
        {
            //TODO: Func. call like this below OR Wheel class fetches data from this class?
            // Also probably should be an interface to a concrete implementation. Same for the NaiveGroundControl below.
            if (_controller.isCanDrive)
                wheel.ApplyForwardForce(forwardAcceleration / 4);
        }
    }
    
    private void ApplyWheelRotation(float steerAngle)
    {
        // Apply steer angle to each wheel
        foreach (var wheel in _wheelArray)
        {
            //TODO: Func. call like this below OR Wheel class fetches data from this class?
            // Also probably should be an interface to a concrete implementation. Same for the NaiveGroundControl below.
            wheel.RotateWheels(steerAngle);
        }
    }

    private float CalcForwardForce(float throttleInput)
    {
        // Throttle
        float forwardAcceleration = 0;

        if (GameManager.InputManager.isBoost)
            forwardAcceleration = GetForwardAcceleration(_controller.forwardSpeedAbs);
        else
            forwardAcceleration = throttleInput * GetForwardAcceleration(_controller.forwardSpeedAbs);

        // Braking
        if (_controller.forwardSpeedSign != Mathf.Sign(throttleInput) && throttleInput != 0)
            forwardAcceleration += -1 * _controller.forwardSpeedSign * 35;
        return forwardAcceleration;
    }

    private float CalculateForwardForce(float input, float speed)
    {
        return  input * GetForwardAcceleration(_controller.forwardSpeedAbs);
    }
    
    private float CalculateSteerAngle()
    {
        var curvature = 1 / GetTurnRadius(_controller.forwardSpeed);
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
            throttle = RoboUtils.Scale(14, 14.1f, 1.6f, 0, speed);
        else if (speed <= (1400 / 100))
            throttle = RoboUtils.Scale(0, 14, 16, 1.6f, speed);

        return throttle;
    }

    static float GetTurnRadius(float speed)
    {
        float forwardSpeed = Mathf.Abs(speed);
        float turnRadius = 0;
        
        var curvature = RoboUtils.Scale(0, 5, 0.0069f, 0.00398f, forwardSpeed);

        if (forwardSpeed >= 500 / 100)
            curvature = RoboUtils.Scale(5, 10, 0.00398f, 0.00235f, forwardSpeed);

        if (forwardSpeed >= 1000 / 100)
            curvature = RoboUtils.Scale(10, 15, 0.00235f, 0.001375f, forwardSpeed);

        if (forwardSpeed >= 1500 / 100)
            curvature = RoboUtils.Scale(15, 17.5f, 0.001375f, 0.0011f, forwardSpeed);

        if (forwardSpeed >= 1750 / 100)
            curvature = RoboUtils.Scale(17.5f, 23, 0.0011f, 0.00088f, forwardSpeed);

        turnRadius = 1 / (curvature * 100);
        return turnRadius;
    }
    
    float _naiveRotationForce = 5;
    float _naiveRotationDampeningForce = -10;
    private void NaiveGroundControl()
    {
        if (_controller.carState != CubeController.CarStates.AllWheelsSurface &&
            _controller.carState != CubeController.CarStates.AllWheelsGround) return;

        // Throttle
        var throttleInput = Input.GetAxis("Vertical");
        float Fx = throttleInput * GetForwardAcceleration(_controller.forwardSpeedAbs);
        _rb.AddForceAtPosition(Fx * transform.forward, _rb.transform.TransformPoint(_rb.centerOfMass),
            ForceMode.Acceleration);

        // Auto dampening
        _rb.AddForce(transform.forward * (5.25f * -Mathf.Sign(_controller.forwardSpeed) * (1 - Mathf.Abs(throttleInput))),
            ForceMode.Acceleration);
        // alternative auto dampening
        //if (throttleInput == 0) _rb.AddForce(transform.forward * (5.25f * -Mathf.Sign(forwardSpeed)), ForceMode.Acceleration); 

        // Steering
        _rb.AddTorque(transform.up * (Input.GetAxis("Horizontal") * _naiveRotationForce), ForceMode.Acceleration);
        _rb.AddTorque(transform.up * (_naiveRotationDampeningForce * (1 - Mathf.Abs(Input.GetAxis("Horizontal"))) *
                                      transform.InverseTransformDirection(_rb.angularVelocity).y), ForceMode.Acceleration);
    }
}