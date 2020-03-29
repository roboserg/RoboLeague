using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CubeSphereCollider : MonoBehaviour
{
    public bool isTouchingSurface = false;
    public float _rayOffset = 0.05f;
    
    CubeController _controller;
    float _rayLen;
    Vector3 _contactPoint;
    
    [HideInInspector]
    // Debug options
    public bool isDrawWheelDisc = false, isDrawRaycast = false;

    private void Awake()
    {
        _controller = GetComponentInParent<CubeController>();
        _rayLen = transform.localScale.x / 2 + _rayOffset;
    }
    
    private void FixedUpdate()
    {
        isTouchingSurface = IsSurfaceContact();
    }

    // Does a wheel touches the ground? Using raycasts, not sphere collider contact point, since no suspention
    bool IsSurfaceContact()
    {
        var isHit = Physics.Raycast(transform.position, -transform.up, out var hit, _rayLen);
        _contactPoint = hit.point;
        return false || isHit;
    }

    private void OnDrawGizmos()
    {
        _rayLen = transform.localScale.x / 2 + _rayOffset;
        var rayEndPoint = transform.position - (transform.up * _rayLen);
        Gizmos.color = Color.red;
        Handles.color = Color.red;

        Vector3 spherePos;
        if (IsSurfaceContact())
        {
            Gizmos.color = Color.green;
            Handles.color = Color.green;
            spherePos = _contactPoint;
        }
        else spherePos = rayEndPoint;

        // Draw vertical lines for ground contact for visual feedback
        if (isDrawRaycast)
        {
            // Draw Raycast ray
            Gizmos.DrawLine(transform.position, rayEndPoint);
            Gizmos.DrawSphere(spherePos, 0.02f);
            // Draw vertical line as ground hit indicators         
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);
        }
    }
}
