using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    public float randomSpeed = 40;
    public float initialForce = 400;
    public float hitMultiplier = 50;
    
    Rigidbody _rb;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            float speed = Random.Range(randomSpeed - 10, randomSpeed + 10);
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(randomCircle.x, Random.Range(-0.5f, 0.5f), randomCircle.y).normalized;
            _rb.velocity = direction * speed;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 desired = new Vector3(0, 12.23f, 0f);
            transform.position = desired;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero; 
        }

        if (Input.GetButtonDown("Select"))
        {
            transform.position = new Vector3(7.76f, 2.98f, 0f);
            _rb.velocity = new Vector3(30, 10,0);
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.CompareTag("Player"))
        {
            {
                float force = initialForce + col.rigidbody.velocity.magnitude * hitMultiplier;
                //Vector3 dir = transform.position - col.contacts[0].point;
                Vector3 dir = transform.position - col.transform.position;
                _rb.AddForce(dir.normalized * force);
            }
        }

        //if (col.gameObject.tag == "Ground")
        //    if (rb.velocity.y > 3)
        //    {
        //    //rb.AddForce(Vector3.up * -downForce);
        //        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - SlowVelocityGround, rb.velocity.z);
        //    }
    }
}
