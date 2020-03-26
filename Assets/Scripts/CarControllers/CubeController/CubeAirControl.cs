using UnityEngine;

public class CubeAirControl : MonoBehaviour
{
    public bool isUseDamperTorque = true;
    Rigidbody _rb;
    CubeController _controller;

    const float Tr = 36.07956616966136f; // torque coefficient for roll
    const float Tp = 12.14599781908070f; // torque coefficient for pitch
    const float Ty = 8.91962804287785f; // torque coefficient for yaw
    const float Dr = -4.47166302201591f; // drag coefficient for roll
    const float Dp = -2.798194258050845f; // drag coefficient for pitch
    const float Dy = -1.886491900437232f; // drag coefficient for yaw
    float _inputRoll = 0, _inputPitch = 0, _inputYaw = 0;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponent<CubeController>();
    }

    void Update()
    {
        _inputYaw = Input.GetAxis("Horizontal");
        _inputPitch = Input.GetAxis("PitchAxis");

        if (Input.GetKey(KeyCode.E) || Input.GetButton("B"))
            _inputRoll = -1;
        else if (Input.GetKey(KeyCode.Q) || Input.GetButton("Y"))
            _inputRoll = 1;
        else
            _inputRoll = 0;

        if (Input.GetButton("LB") || Input.GetKey(KeyCode.LeftShift))
        {
            _inputRoll = -_inputYaw;
            _inputYaw = 0;
        }
    }

    private void FixedUpdate()
    {
        if (_controller.numWheelsSurface < 3) 
        {
            // roll
            _rb.AddTorque(Tr * _inputRoll * transform.forward, ForceMode.Acceleration);
            if(isUseDamperTorque) _rb.AddTorque(Dr * transform.InverseTransformDirection(_rb.angularVelocity).z * transform.forward, ForceMode.Acceleration);

            // pitch
            _rb.AddTorque(Tp * _inputPitch * transform.right, ForceMode.Acceleration);
            if(isUseDamperTorque) _rb.AddTorque(transform.right * (Dp * (1 - Mathf.Abs(_inputPitch)) * transform.InverseTransformDirection(_rb.angularVelocity).x), ForceMode.Acceleration);

            //yaw
            _rb.AddTorque(Ty * _inputYaw * transform.up, ForceMode.Acceleration);
            if(isUseDamperTorque) _rb.AddTorque(transform.up * (Dy * (1 - Mathf.Abs(_inputYaw)) * transform.InverseTransformDirection(_rb.angularVelocity).y), ForceMode.Acceleration);
        }
    }
}
