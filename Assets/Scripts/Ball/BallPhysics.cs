using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public bool isFreeze = false;
    public float RandomSpeed = 10;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        if (n.magnitude > 0.0f)
        {
            Vector3 L = p - x;

            float m_reduced = 1.0f / ((1.0f / m) + Vector3.Dot(L, L) / I);

            Vector3 v_perp =  Mathf.Min(Vector3.Dot(v, n), 0.0f) * n;
            Vector3 v_para = v - v_perp - Vector3.Cross(L, w);

            float ratio = (v_perp.magnitude) / Mathf.Max(v_para.magnitude, 0.0001f);

            Vector3 J_perp = -(1.0f + restitution) * m * v_perp;
            Vector3 J_para = -Mathf.Min(1.0f, mu * ratio) * m_reduced * v_para;

            Vector3 J = J_perp + J_para;

            w += Vector3.Cross(L, J) / I;
            v += (J / m) + drag * v * dt;
            x += v * dt;

            float penetration = collision_radius - Vector3.Dot(x - p, n);
            if (penetration > 0.0f)
            {
                x += 1.001f * penetration * n;
            }
        }
        else
        {
            v += (drag * v + gravity) * dt;
            x += v * dt;
            
        }

        transform.position = x/100;
        rb.angularVelocity = w;
        time += Time.fixedDeltaTime;

        n = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        p = collision.contacts[0].point*100;    // contact point
        n = collision.contacts[0].normal;   // contact.direction
        //lastVelMagn = rb.velocity.magnitude;
        VelMagnList.Add(-Vector3.Dot(n, v/100));
        contactPointList.Add(p/100);
        normalList.Add(n);

        if(isFreeze) freeze();
    }

    private void OnCollisionStay(Collision collision)
    {
        //p = collision.contacts[0].point * 100;    // contact point
        //n = collision.contacts[0].normal;   // contact.direction

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            float speed = Random.Range(RandomSpeed-10, RandomSpeed+10) * 100;
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(randomCircle.x, Random.Range(-0.1f, 0.1f), randomCircle.y).normalized;
            v = direction * speed;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Vector3 desired = 
            transform.position = Vector3.zero;
            x = Vector3.zero;
            v = Vector3.zero;
            w = Vector3.zero;
        }
    }

    void freeze()
    {
        v = Vector3.zero;
        gravity = Vector3.zero;
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < contactPointList.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(contactPointList[i], 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(contactPointList[i], contactPointList[i] + normalList[i].normalized * VelMagnList[i]);
            Gizmos.DrawSphere(contactPointList[i] + normalList[i].normalized * VelMagnList[i], 0.2f);
        }
    }

    Vector3 p = Vector3.zero;
    Vector3 n = Vector3.zero;

    private Vector3 x; //Vector3 x = new Vector3(0.0f, 0.0f, 1.1f * collision_radius);
    public Vector3 v = Vector3.zero;
    private Vector3 w = Vector3.zero;

    Rigidbody rb;
    float time = 0f;

    const float restitution = 0.6f;  // bounciness
    const float drag = -0.0305f;      // air drag
    const float mu = 2.0f;              // coulomb friction coefficient = static coefficient
    const float v_max = 4000.0f;
    const float w_max = 6.0f;
    const float m = 30.0f;
    Vector3 gravity = new Vector3(0, -650f, 0);

    //ball radius
    const float soccar_radius = 91.25f;
    const float soccar_collision_radius = 93.15f;
    const float radius = soccar_radius;
    const float collision_radius = soccar_collision_radius;
    const float I = 0.4f * m * soccar_radius * soccar_radius;

    float lastVelMagn = 0f;
    List<float> VelMagnList = new List<float>();
    List<Vector3> contactPointList = new List<Vector3>();
    List<Vector3> normalList = new List<Vector3>();
}