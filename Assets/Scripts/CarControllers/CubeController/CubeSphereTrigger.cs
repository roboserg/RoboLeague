using UnityEngine;

public class CubeSphereTrigger : MonoBehaviour
{
    public bool isDebug = false;
    public bool isDrawRay = false;
    public bool isTouchingSurface = false;
    CubeController _controller;
    Collider _col;

    void Start()
    {
        _controller = GetComponentInParent<CubeController>();
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Map"))
    //        isTouchingSurface = true;
    //    else
    //        isTouchingSurface = false;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Map"))
        {
            _controller.numWheelsSurface++;
            isTouchingSurface = true;
        }
        if (isDebug)
            print(transform.name + " is in contact with " + other.tag);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Map"))
        {
            _controller.numWheelsSurface--;
            isTouchingSurface = false;
        }
        if (isDebug)
            print(transform.name + " is no longer in contact with " + other.tag);
    }

    private void OnDrawGizmos()
    {
        if (!isDrawRay) return;

        Gizmos.color = isTouchingSurface ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
    }
}
