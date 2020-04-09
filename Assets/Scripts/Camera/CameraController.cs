using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool isWatchBall = false;
    public float distanceToBall = 20;

    [Header("Ballcam on settings")]
    public float cameraDist = -9f;
    public float cameraHeight = 3f;
    public float cameraAngle = 2.3f;
    public float stiffnessPosition = 50;
    public float stiffnessAngle = 30;

    [Header("Ballcam off settings")]
    public float cameraDistOff = -9f;
    public float cameraHeightOff = 3f;
    public float cameraAngleOff = 2.3f;
    public float stiffnessPositionOff = 50;
    public float stiffnessAngleOff = 30;

    Transform _ball, _car;
    bool _isBallCam = true;
    void Start()
    {
        _ball = GameObject.FindGameObjectWithTag("Ball").transform;
        _car = GameObject.FindGameObjectWithTag("CarCameraTarget").transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("X"))
            _isBallCam = !_isBallCam;
        if (Input.GetKeyDown(KeyCode.V))
            isWatchBall = !isWatchBall;
    }
    
    private void FixedUpdate()
    {
        if (_isBallCam)
        {
            MoveToTarget(stiffnessPosition);
            LookAtTarget(stiffnessAngle);
        }
        else
        {
            MoveToTarget(stiffnessPositionOff);
            LookAtTarget(stiffnessAngleOff);
        }
    }
    void MoveToTarget(float stiffnessPos)
    {
        var desiredPosition = transform.position;

        if (isWatchBall)
        {
            var direction = (transform.position - _ball.position).normalized;
            desiredPosition = _ball.position + transform.forward * -distanceToBall;
            if (desiredPosition.y < -7f)
                desiredPosition.y = -7f;
        }

        else if (_isBallCam && !isWatchBall)
        {
            var offset = new Vector3(0, cameraHeight, cameraDist);
            desiredPosition = _car.position + transform.forward * cameraDist + transform.right * offset.x + transform.up * offset.y;
            if (desiredPosition.y < _car.position.y - 1)
                desiredPosition.y = _car.position.y - 0.9f;
        }

        else if (!isWatchBall)
        {
            var offset = new Vector3(0, cameraHeightOff, cameraDistOff);
            desiredPosition = _car.position + _car.forward * offset.z + _car.right * offset.x + _car.up * offset.y;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, stiffnessPos * Time.deltaTime);
    }

    void LookAtTarget(float stiffnessAngle)
    {
        Vector3 angleOffset = new Vector3(0, cameraAngle, 0);

        if(isWatchBall)
        {
            Vector3 desiredAngle = _ball.position - transform.position + angleOffset;
            //DesiredAngle.y = transform.rotation.y + CameraAngle;
            Quaternion _rot = Quaternion.LookRotation(desiredAngle, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, _rot, stiffnessAngle * Time.deltaTime);
            return;
        }

        if (_isBallCam)
        {
            Vector3 desiredAngle = _ball.position - transform.position + angleOffset;
            desiredAngle.y = transform.rotation.y + cameraAngle;
            //DesiredAngle.z = 0;
            Quaternion rot = Quaternion.LookRotation(desiredAngle, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, stiffnessAngle * Time.deltaTime);
        }
        else
        {
            angleOffset = new Vector3(0, cameraAngleOff, 0);
            Vector3 desiredAngle = _car.position - transform.position + angleOffset;
            Quaternion rot = Quaternion.LookRotation(desiredAngle, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, stiffnessAngle * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, Ball.position);
    }

    RaycastHit Raycast(Vector3 origin, Vector3 direction, float maxDist)
    {
        Physics.Raycast(origin, direction, out RaycastHit hit, maxDist);
        return hit;
    }
    bool isRaycast(Vector3 origin, Vector3 direction, float maxDist)
    {
        return Physics.Raycast(origin, direction, maxDist);
    }
}