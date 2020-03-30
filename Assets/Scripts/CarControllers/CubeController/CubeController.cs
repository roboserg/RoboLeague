using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [Header("Car State")]
    public bool isAllWheelsSurface = false;
    public bool isCanDrive;
    public float forwardSpeed = 0, forwardSpeedSign = 0, forwardSpeedAbs = 0;
    public int numWheelsSurface;
    public bool isBodySurface;
    public CarStates carState;
    public enum CarStates
    {
        AllWheelsGround,
        Air,
        AllWheelsSurface,
        SomeWheelsSurface,
        BodySideGround,
        BodyGroundDead
    }

    [Header("Other")]
    public Transform cogLow;

    const float MaxSpeedBoost = 2300 / 100;

    Rigidbody _rb;
    private CubeGroundControl _groundControl;
    static readonly GUIStyle Style = new GUIStyle();
    CubeSphereCollider[] _sphereColliders;
    public GameObject sceneViewFocusObject;
    public CubeParticleSystem cubeParticleSystem;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = cogLow.localPosition;
        _rb.maxAngularVelocity = 5.5f;

        _sphereColliders = GetComponentsInChildren<CubeSphereCollider>();
        _groundControl = GetComponent<CubeGroundControl>();
        Style.normal.textColor = Color.red;
        Style.fontSize = 25;
        Style.fontStyle = FontStyle.Bold;
        
        // Lock scene view camera to the car
        Selection.activeGameObject = sceneViewFocusObject;
        SceneView.lastActiveSceneView.FrameSelected(true);

        // Activate ParticleSystems GameObject
        if (Resources.FindObjectsOfTypeAll<CubeParticleSystem>()[0] != null)
            Resources.FindObjectsOfTypeAll<CubeParticleSystem>()[0].gameObject.SetActive(true);
    }

    private void Update()
    {
        SetCarState();
        //TODO:  limit _rb.velocity.magnitude to < maxSpeedBoost
    }

    void FixedUpdate()
    {
        UpdateCarVariables();
        Boosting();
        DownForce();
        //SetDrag();
    }

    private void UpdateCarVariables()
    {
        forwardSpeed = Vector3.Dot(_rb.velocity, transform.forward);
        //if(Mathf.Abs(forwardSpeed) < 0.1f)
        forwardSpeed = (float) System.Math.Round(forwardSpeed, 2);
        forwardSpeedAbs = Mathf.Abs(forwardSpeed);
        forwardSpeedSign = Mathf.Sign(forwardSpeed);
    }

    void Boosting()
    {
        if (GameManager.InputManager.isBoost)
        {
            if (forwardSpeed < MaxSpeedBoost)
            {
                _rb.AddForce(transform.forward * 991 / 100, ForceMode.Acceleration);
                //_groundControl.throttleInput = 1;
            }
        }
    }
    
    void SetCarState()
    {
        int temp = 0;
        foreach (var c in _sphereColliders)
        {
            if (c.isTouchingSurface)
                temp++;
        }
        numWheelsSurface = temp;
        isAllWheelsSurface = numWheelsSurface >= 3 ? true : false;

        // All wheels are touching the ground
        if (isAllWheelsSurface)
            carState = CarStates.AllWheelsSurface;

        // Some wheels are touching the ground, but not the body
        if (!isAllWheelsSurface && !isBodySurface)
            carState = CarStates.SomeWheelsSurface;

        // We are lying on our side
        if (isBodySurface && !isAllWheelsSurface)
            carState = CarStates.BodySideGround;

        // All wheels on the ground
        if (isAllWheelsSurface && Vector3.Dot(Vector3.up, transform.up) > 0.95f)
            carState = CarStates.AllWheelsGround;

        // He is dead Jimmy!
        if (isBodySurface && Vector3.Dot(Vector3.up, transform.up) < -0.95f)
            carState = CarStates.BodyGroundDead;

        // In the air
        if (!isBodySurface && numWheelsSurface == 0)
        {
            carState = CarStates.Air;
            //rb.centerOfMass = Vector3.zero;
        }
        
        isCanDrive = false || (carState == CubeController.CarStates.AllWheelsSurface || carState == CubeController.CarStates.AllWheelsGround);
    }
    
    private void SetDrag()
    {
        if (carState == CarStates.Air)
        {
            _rb.drag = 0;
            _rb.angularDrag = 0;
        }
        else
        {
            _rb.drag = 1;
            _rb.angularDrag = 1;
        }
    }
    
    void DownForce()
    {
        if (carState == CarStates.AllWheelsSurface || carState == CarStates.AllWheelsGround)
            _rb.AddForce(-transform.up * 5, ForceMode.Acceleration);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(cogLow.transform.position, 0.03f);
        if (_rb == null) return;
        // Draw CG
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(_rb.transform.TransformPoint(_rb.centerOfMass), 0.03f);
    }
    void OnGUI()
    {
        GUI.Label(new Rect(30.0f, 20.0f, 150, 130), $"{forwardSpeed:F2} m/s {forwardSpeed * 100:F0} uu/s", Style);
        //GUI.Label(new Rect(30.0f, 40.0f, 150, 130), string.Format("turnRadius: {0:F2} m curvature: {1:F4}", turnRadius, curvature), style);
        GUI.Label(new Rect(30.0f, 60.0f, 150, 130), $"car state: {carState.ToString()}", Style);
    }

    #region Utils

    public static float Scale(float oldMin, float oldMax, float newMin, float newMax, float oldValue)
    {
        float oldRange = (oldMax - oldMin);
        float newRange = (newMax - newMin);
        float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;

        return (newValue);
    }
    public static void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
    #endregion
}