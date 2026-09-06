using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAirControl : MonoBehaviour
{
    [HideInInspector]
    public float inputRoll = 0, inputPitch = 0, inputYaw = 0;
    [HideInInspector]
    public bool inputBoost = false;
    public float boostMultiplier = 1;
    Rigidbody _rb;
    #region Torque Coefficients for rotation and drag
    const float Tr = 36.07956616966136f; // torque coefficient for roll
    const float Tp = 12.14599781908070f; // torque coefficient for pitch
    const float Ty = 8.91962804287785f; // torque coefficient for yaw
    const float Dr = -4.47166302201591f; // drag coefficient for roll
    const float Dp = -2.798194258050845f; // drag coefficient for pitch
    const float Dy = -1.886491900437232f; // drag coefficient for yaw
    #endregion
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        //_controller = GetComponent<CubeController>();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        // roll
        _rb.AddTorque(Tr * inputRoll * transform.forward, ForceMode.Acceleration);
        _rb.AddTorque(Dr * transform.InverseTransformDirection(_rb.angularVelocity).z * transform.forward, ForceMode.Acceleration);

        // pitch
        _rb.AddTorque(Tp * inputPitch * transform.right, ForceMode.Acceleration);
        _rb.AddTorque(transform.right * (Dp * (1 - Mathf.Abs(inputPitch)) * transform.InverseTransformDirection(_rb.angularVelocity).x), ForceMode.Acceleration);

        //yaw
        _rb.AddTorque(Ty * inputYaw * transform.up, ForceMode.Acceleration);
        _rb.AddTorque(transform.up * (Dy * (1 - Mathf.Abs(inputYaw)) * transform.InverseTransformDirection(_rb.angularVelocity).y), ForceMode.Acceleration);
        
        //boost
        if(inputBoost)
            _rb.AddForce(transform.forward * (9.91f * boostMultiplier), ForceMode.Acceleration);
    }
    
}
