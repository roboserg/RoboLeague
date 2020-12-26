using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script defines a car, which has health, speed, rotation speed, damage, and other attributes related to the car's behaviour. It also defines AI controls when the car is not player-controlled.
/// </summary>
public class ECCCar : MonoBehaviour
{
    // Various varialbes for quicker access
    static Transform targetPlayer;

    internal RaycastHit groundHitInfo;
    internal Vector3 groundPoint;
    internal Vector3 forwardPoint;
    internal float forwardAngle;
    internal float rightAngle;
    internal Rigidbody rb;

    [Tooltip("The speed of the player, how fast it moves player. The player moves forward constantly")]
    public float speed = 10;

    [Tooltip("How quickly the player car rotates, in both directions")]
    public float rotateSpeed = 200;
    internal float currentRotation = 0;

    [Tooltip("The slight extra rotation that happens to the car as it turns, giving a drifting effect")]
    public float driftAngle = 50;

    [Tooltip("The slight side tilt that happens to the car chassis as the car turns, making it lean inwards or outwards from the center of rotation")]
    public float leanAngle = 10;

    [Tooltip("The chassis object of the car which leans when the car rotates")]
    public Transform chassis;

    [Tooltip("The wheels of the car which rotate based on the speed of the car. The front wheels also rotate in the direction the car is turning")]
    public Transform[] wheels;

    [Tooltip("The front wheels of the car also rotate in the direction the car is turning")]
    public int frontWheels = 2;

    internal int index;

    [Header("My stuff")]
    public LayerMask groundLayer;
    public Transform groundObject;
    GameObject playerObject;
    public float forwardToque = 10;
    public float boostTorque = 10;
    public float downForce = 4;
    public Transform cog;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //fffrb.centerOfMass = cog.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 5 + transform.forward * 1.0f, -10 * Vector3.up, out hit, 100, groundLayer)) 
            forwardPoint = hit.point;
        transform.Find("Base").LookAt(forwardPoint);// + transform.Find("Base").localPosition);
    }

    // This function runs whenever we change a value in the component
    private void OnValidate()
    {
        // Limit the maximum number of front wheels to the actual front wheels we have
        frontWheels = Mathf.Clamp(frontWheels, 0, wheels.Length);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rb.velocity.magnitude);
        //DetectGround();
        //rb.AddForce(Vector3.up * -downForce);
        rb.AddForce(Input.GetAxis("Vertical") * transform.forward * forwardToque);
        Rotate(Input.GetAxis("Horizontal"));

        //transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);

        // Boost
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(transform.forward * boostTorque);
        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    //Vector3 desired = 
        //    GameObject.FindGameObjectWithTag("Ball").transform.position = transform.position + Vector3.up*2;
        //}

        if (rb.velocity.magnitude > 23)
        {
            rb.velocity = rb.velocity.normalized * 23;
        }
    }

    /// <summary>
    /// Rotates the car either left or right, and applies the relevant lean and drift effects
    /// </summary>
    /// <param name="rotateDirection"></param>
    public void Rotate(float rotateDirection)
    {
        //transform.localEulerAngles = new Vector3(Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles.x, transform.localEulerAngles.y, Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles.z);
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal);

        // If the car is rotating either left or right, make it drift and lean in the direction its rotating
        if (rotateDirection != 0)
        {
            //transform.localEulerAngles = Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles + Vector3.up * currentRotation;

            // Rotate the car based on the control direction
            transform.localEulerAngles += Vector3.up * rotateDirection * rotateSpeed * Time.deltaTime;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

            //transform.eulerAngles = new Vector3(rightAngle, transform.eulerAngles.y, forwardAngle);

            currentRotation += rotateDirection * rotateSpeed * Time.deltaTime;

            //if (currentRotation > 360) currentRotation -= 360;
            ////print(forwardAngle);
            //// Make the base of the car drift based on the rotation angle
            //transform.Find("Base").localEulerAngles = new Vector3(rightAngle, Mathf.LerpAngle(transform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle + Mathf.Sin(Time.time * 50) * hurtDelayCount * 50, Time.deltaTime), 0);//  Mathf.LerpAngle(transform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

            // Make the chassis lean to the sides based on the rotation angle
            if (chassis) 
                chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, rotateDirection * leanAngle, Time.deltaTime);//  Mathf.LerpAngle(transform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

            // Go through all the wheels making them spin, and make the front wheels turn sideways based on rotation
            for (index = 0; index < wheels.Length; index++)
            {
                // Turn the front wheels sideways based on rotation
                if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime * 10);

                // Spin the wheel
                wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * rb.velocity.magnitude * 20, Space.Self);
            }
        }
        else // Otherwise, if we are no longer rotating, straighten up the car and front wheels
        {
            // Return the base of the car to its 0 angle
            transform.Find("Base").localEulerAngles = Vector3.up * Mathf.LerpAngle(transform.Find("Base").localEulerAngles.y, 0, Time.deltaTime * 5);

            // Return the chassis to its 0 angle
            if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, 0, Time.deltaTime * 5);//  Mathf.LerpAngle(transform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

            // Go through all the wheels making them spin faster than when turning, and return the front wheels to their 0 angle
            for (index = 0; index < wheels.Length; index++)
            {
                // Return the front wheels to their 0 angle
                if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, 0, Time.deltaTime * 5);

                // Spin the wheel faster
                wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * transform.InverseTransformDirection(rb.velocity).z * 300, Space.Self);
                //drawString(rb.velocity.z.ToString(), transform.position + Vector3.up, Color.black);
                //drawString(transform.InverseTransformDirection(rb.velocity).z.ToString(), transform.position + Vector3.up*2, Color.black);
            }
        }
    }

    /// <summary>
    /// Detects the terrain under the car and aligns it to it
    /// </summary>
    public void DetectGround()
    {
        // Cast a ray to the ground below
        Ray carToGround = new Ray(transform.position, -Vector3.up * 20);

        // Detect terrain under the car
        if (Physics.Raycast(carToGround, out groundHitInfo, 200, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, groundHitInfo.point.y, transform.position.z);
        }

        // Set the position of the car along the terrain
        transform.position = new Vector3(transform.position.x, groundHitInfo.point.y + 0.1f, transform.position.z);


        // Detect a point along the terrain in front of the car, so that we can make the car rotate accordingly
        if (Physics.Raycast(transform.position + Vector3.up * 5 + transform.forward * 1.0f, -10 * Vector3.up, out RaycastHit hit, 100, groundLayer))
        {
            forwardPoint = hit.point;
        }
        else if (groundObject && groundObject.gameObject.activeSelf == true)
        {
            forwardPoint = new Vector3(transform.position.x, groundObject.position.y, transform.position.z);
        }

        // Make the car look at the point in front of it along the terrain
        transform.Find("Base").LookAt(forwardPoint);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cog.position, 0.05f);

        if (Physics.Raycast(transform.position + Vector3.up * 5 + transform.right * 1.0f, 10 * Vector3.up, out RaycastHit hit, 100, groundLayer))
            Gizmos.DrawRay(transform.position, hit.transform.position);

        //Gizmos.DrawRay(transform.position, -Vector3.up * 20);
        //Gizmos.DrawSphere(, 0.2f);
        //Gizmos.DrawSphere(forwardPoint, 0.5f);
    }

    static public void drawString(string text, Vector3 worldPos, Color? colour = null, float oX = 0, float oY = 0)
    {

#if UNITY_EDITOR
        UnityEditor.Handles.BeginGUI();

        var restoreColor = GUI.color;

        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        {
            GUI.color = restoreColor;
            UnityEditor.Handles.EndGUI();
            return;
        }

        UnityEditor.Handles.Label(TransformByPixel(worldPos, oX, oY), text);

        GUI.color = restoreColor;
        UnityEditor.Handles.EndGUI();
#endif
    }

    static Vector3 TransformByPixel(Vector3 position, float x, float y)
    {
        return TransformByPixel(position, new Vector3(x, y));
    }

    static Vector3 TransformByPixel(Vector3 position, Vector3 translateBy)
    {
#if UNITY_EDITOR
        Camera cam = UnityEditor.SceneView.currentDrawingSceneView.camera;
        return cam ? cam.ScreenToWorldPoint(cam.WorldToScreenPoint(position) + translateBy) : position;
#endif
        return Vector3.zero;
    }
}