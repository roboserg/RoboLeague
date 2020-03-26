using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSphereCollider : MonoBehaviour
{
    public bool isTouchingSurface = false;
    CubeController controller;
    private void Awake()
    {
        controller = GetComponentInParent<CubeController>();
    }

    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        float rayLen = (transform.localScale.x / 2) + 0.05f;

        Gizmos.color = Color.cyan;

        isTouchingSurface = false;
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, rayLen))
        {
            isTouchingSurface = true;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.02f);
        }
        else
        {
            Gizmos.DrawSphere(transform.position - (transform.up * rayLen), 0.02f);  
        }
        Gizmos.DrawLine(transform.position, transform.position - (transform.up * rayLen));
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
    }
}
