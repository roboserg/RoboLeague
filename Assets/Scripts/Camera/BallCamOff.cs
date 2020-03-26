
using UnityEngine;

public class BallCamOff : MonoBehaviour
{
    public Transform Ball;
    public void LookAtTarget()
    {
        Vector3 _lookDir = objToFollow.position - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDir, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
    }

    public void MoveToTarget()
    {
        Vector3 _targetPos = objToFollow.position + objToFollow.forward * offset.z + objToFollow.right * offset.x + objToFollow.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        LookAtTarget();
        MoveToTarget();
    }

    public Transform objToFollow;
    public Vector3 offset = new Vector3 (0, 1.8f, -5.6f);
    public float followSpeed = 10;
    public float lookSpeed = 10;
}
