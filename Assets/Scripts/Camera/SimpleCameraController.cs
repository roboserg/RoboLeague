
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public Transform Ball;
    public void LookAtTarget()
    {
        transform.LookAt(Ball);
        //Vector3 _lookDir = objToFollow.position - transform.position;
        //Quaternion _rot = Quaternion.LookRotation(_lookDir, Vector3.up);
        //transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);


    }

    public void MoveToTarget()
    {
        Vector3 _targetPos = objToFollow.position + transform.forward * offset.z + transform.right * offset.x + transform.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);

        //Vector3 _targetPos = objToFollow.position;
        //_targetPos += objToFollow.position + objToFollow.forward * offset.z + objToFollow.right * offset.x + objToFollow.up * offset.y;


    }

    private void FixedUpdate()
    {
        LookAtTarget();
        MoveToTarget();
    }

    public Transform objToFollow;
    public Vector3 offset;
    public float followSpeed = 10;
    public float lookSpeed = 10;
}
