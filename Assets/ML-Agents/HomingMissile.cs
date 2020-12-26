using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// An object that will move towards an object marked with the tag 'targetTag'. 
/// </summary>
public class HomingMissile : MonoBehaviour
{
    public Transform target;
    public float turnSpeed;
    public int distance;
    public float timeInterval = 5;
    private float _nextActionTime = 2;
    public Vector2 rocketVelocity= new Vector2(50, 100);
    Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        InitPosition(target.position);
    }

    private void FixedUpdate()
    {
        if (Time.time > _nextActionTime ) 
        {
            _nextActionTime += timeInterval;
            InitPosition(target.position);
        }

        _rb.velocity = transform.forward * (Random.Range(rocketVelocity.x, rocketVelocity.y));
        var rocketTargetRotation = Quaternion.LookRotation(target.position - transform.position);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rocketTargetRotation, turnSpeed));
    }
    
    public void InitPosition(Vector3 pos)
    {
        transform.position = pos + Random.onUnitSphere * distance;
        if(transform.position.y < 1)
            transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
    }

    private void OnCollisionEnter(Collision other)
    {
        InitPosition(target.position);
    }
}