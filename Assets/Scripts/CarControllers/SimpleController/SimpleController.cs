using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    public float velForward, velMagn;
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        velForward = Vector3.Dot(_rb.velocity, transform.forward);
        velForward = (float)Math.Round(velForward, 2);
        velMagn = _rb.velocity.magnitude;
    }
}
