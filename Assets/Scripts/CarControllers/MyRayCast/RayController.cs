using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayController : MonoBehaviour
{
    public float Torque;
    public float downForce = 0;
    public float jumpForce = 1000;
    public float maxSpeedNoBoost = 1410 / 100;
    public float maxSpeedBoost = 2300 / 100;
    float supersonicThreshold = 2200 / 100;
    public RayWheel[] wheels;
    public Transform cog;

    float wheelBase;
    float rearTrack;
    float turnRadius;
    float steerInput;

    float curvature = 0;

    static GUIStyle style = new GUIStyle();

    Rigidbody rb;
    public float forwardSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = cog.localPosition;

        rearTrack = Mathf.Abs(GameObject.Find("FR").transform.position.z - GameObject.Find("FL").transform.position.z);
        wheelBase = Mathf.Abs(GameObject.Find("FR").transform.position.x - GameObject.Find("BR").transform.position.x);

        if (wheels == null)
            Debug.LogError("Assign wheels!");

        style.normal.textColor = Color.red;
        style.fontSize = 25;
        style.fontStyle = FontStyle.Bold;

        cameraFov = GameObject.FindObjectOfType<CameraFov>();
        FireParticlesSystem.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
            isCanJump = false;

        if (Input.GetMouseButtonUp(1))
            isJumpReleased = true;
    }
    float Timer = 0;
    void FixedUpdate()
    {
        Steering();

        Coasting();

        Jumping();

        Boosting();

        CheckGround();

        //DownForce();
        //Debug.Log(Vector3.Dot(Vector3.up, transform.up));
    }

    public float ackermannAngleLeft, ackermannAngleRight;
    void Steering()
    {
        steerInput = Input.GetAxis("Horizontal");
        forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
        turnRadius = GetTurnRadius(Mathf.Abs(forwardSpeed)) * 0.6f;
        rearTrack = 0;
        if (steerInput > 0)
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
        }
        else if (steerInput < 0)
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
        }
        else if (steerInput == 0)
        {
            ackermannAngleLeft = 0;
            ackermannAngleRight = 0;
        }

        foreach (RayWheel w in wheels)
        {
            if (w.wheelFL)
                w.steerAngle = ackermannAngleLeft;
            if (w.wheelFR)
                w.steerAngle = ackermannAngleRight;
        }
    }

    public ParticleSystem SuperSonicParticlesSystem, BoostParticlesSystem;
    public GameObject FireParticlesSystem;
    CameraFov cameraFov;
    bool isBoostAnimationPlaying = false;   
    void Boosting()
    {
        if (Input.GetMouseButton(0))
        {
            rb.drag = 0;
            if (isBoostAnimationPlaying == false)
            {
                BoostParticlesSystem.Play();
                FireParticlesSystem.SetActive(true);
                isBoostAnimationPlaying = true;
            }

            if (forwardSpeed < maxSpeedBoost)
                rb.AddForce(transform.forward * 991 / 100, ForceMode.Acceleration);
        }
        else if (!Input.GetMouseButton(0))
        {
            BoostParticlesSystem.Stop();
            FireParticlesSystem.SetActive(false);
            isBoostAnimationPlaying = false;
        }

        // Set FOV and Wind effect
        if (forwardSpeed >= supersonicThreshold)
        {
            cameraFov.SetCamFOV(90);
            SuperSonicParticlesSystem.Play();
        }
        else
        {
            cameraFov.SetCamFOV(80);
            SuperSonicParticlesSystem.Stop();
        }
    }

    void DownForce()
    {
        // if(isGround())
        rb.AddForce(-transform.up * downForce);
    }

    void Coasting()
    {
        // Limit speed no boost
        // if(isGround())
        //if (rb.velocity.magnitude > maxSpeedNoBoost)
        //    rb.velocity = rb.velocity.normalized * maxSpeedNoBoost;

        rb.drag = 0;
        if (Input.GetAxis("Vertical") == 0 && Mathf.Abs(forwardSpeed) > 1 && !Input.GetKey(KeyCode.LeftShift) && isWheelsSurface)
        {

            if (forwardSpeed > 0)
                rb.AddForce(-transform.forward * 525f / 100 * rb.mass);
            else
                rb.AddForce(transform.forward * 525f / 100 * rb.mass);

        }
        else if (Input.GetAxis("Vertical") == 0 && Mathf.Abs(forwardSpeed) < 1 && !Input.GetKey(KeyCode.LeftShift) && isWheelsSurface)
            rb.drag = 16;
    }

    bool isFirstJumpTick = true;
    bool isCanJump = true;
    float jumpTimer = 0;
    public bool isJumpingRightNow = false;
    bool isJumpReleased = true;
    void Jumping()
    {
        //Debug.Log("isJumpReleased: " + isJumpReleased);
        if (isWheelsSurface)
        {
            jumpTimer = 0;
            isFirstJumpTick = true;
            //ClearConsole();
            isCanJump = isJumpReleased;
        }
        isJumpingRightNow = false;
        if (isCanJump)
        {
            if (Input.GetMouseButton(1) && isFirstJumpTick)
            {
                isJumpingRightNow = true;
                rb.AddForceAtPosition(transform.up * 291.667f / 100, rb.transform.TransformPoint(rb.centerOfMass), ForceMode.VelocityChange);
                isFirstJumpTick = false;
                jumpTimer += Time.fixedDeltaTime;
            }
            else if (Input.GetMouseButton(1) && jumpTimer <= 0.2f && isCanJump)
            {
                isJumpReleased = false;
                rb.AddForceAtPosition(transform.up * 1458f / 100, rb.transform.TransformPoint(rb.centerOfMass), ForceMode.Acceleration);
                jumpTimer += Time.fixedDeltaTime;
                isJumpingRightNow = true;
            }
        }

    }

    #region Utils
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    float GetTurnRadius(float forwardSpeed)
    {
        float turnRadius = 0;
        curvature = scale(0, 5, 0.0069f, 0.00398f, forwardSpeed);

        if (forwardSpeed >= 500 / 100)
            curvature = scale(5, 10, 0.00398f, 0.00235f, forwardSpeed);

        if (forwardSpeed >= 1000 / 100)
            curvature = scale(10, 15, 0.00235f, 0.001375f, forwardSpeed);

        if (forwardSpeed >= 1500 / 100)
            curvature = scale(15, 17.5f, 0.001375f, 0.0011f, forwardSpeed);

        if (forwardSpeed >= 1750 / 100)
            curvature = scale(17.5f, 23, 0.0011f, 0.00088f, forwardSpeed);

        turnRadius = 1 / (curvature * 100);
        return turnRadius;
    }
    void OnGUI()
    {
        GUI.Label(new Rect(30.0f, 20.0f, 150, 130), string.Format("{0:F2} m/s {1:F0} uu/s", forwardSpeed, forwardSpeed * 100), style);
        GUI.Label(new Rect(30.0f, 40.0f, 150, 130), string.Format("turnRadius: {0:F2} m curvature: {1:F4}", turnRadius, curvature), style);
        GUI.Label(new Rect(30.0f, 60.0f, 150, 130), string.Format("ackerLeft: {0:F4} m ackerRight: {1:F4}", ackermannAngleLeft, ackermannAngleRight), style);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(rb.centerOfMass + transform.localPosition, 0.05f);
        Gizmos.color = Color.green;
        if(rb != null)
            Gizmos.DrawSphere(rb.transform.TransformPoint(rb.centerOfMass), 0.05f);
    }
    public bool isWheelsSurface = false;
    void CheckGround()
    {
        isWheelsSurface = true;
        foreach (RayWheel w in wheels)
        {
            isWheelsSurface = isWheelsSurface && w.isWheelSurface;
        }
    }

    public bool isLyingGround = false;
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Roof on the ground!");
            if (Vector3.Dot(Vector3.up, transform.up) < -0.99f)
            {
                //Debug.Log("Lying on the ground");
                isLyingGround = true;
            }
        }
    }
    static void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
    #endregion
}
