using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float RandomSpeed = 50;
    public float InitialForce = 400;
    public float HitMultiplier = 50;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            float speed = Random.Range(RandomSpeed - 10, RandomSpeed + 10);
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(randomCircle.x, Random.Range(-0.5f, 0.5f), randomCircle.y).normalized;
            rb.velocity = direction * speed;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 desired = new Vector3(0, 12.23f, 0f);
            transform.position = desired;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; 
        }

        if (Input.GetButtonDown("Select"))
        {
            transform.position = new Vector3(7.76f, 2.98f, 0f);
            rb.velocity = new Vector3(30, 10,0);
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.CompareTag("Player"))
        {
            {
                float force = InitialForce + col.rigidbody.velocity.magnitude * HitMultiplier;
                //Vector3 dir = transform.position - col.contacts[0].point;
                Vector3 dir = transform.position - col.transform.position;
                rb.AddForce(dir.normalized * force);
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
