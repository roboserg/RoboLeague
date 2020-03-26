using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float Fx = Input.GetAxis("Vertical") * 5;
        rb.AddForceAtPosition(Fx * transform.forward, transform.position + rb.centerOfMass, ForceMode.Acceleration);
    }
}
