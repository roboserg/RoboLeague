using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public float Speed = 40f;
    public float AerialStrength = 35f;
    public float JumpStrength = 300f;
    public float JumpFlip = 30f;
    public float FallForce = 100f;
    public float AirMovementSlowdown = 3f;
    public GameObject Ball;
    public float DownForceAir = 20;

    Rigidbody rb;
    bool isOnTheGround = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Vertical Movement
        
        float moveVertical = Input.GetAxis("Vertical");
        if (!isOnTheGround)
            moveVertical = moveVertical / AirMovementSlowdown;
        Vector3 DirectionToBall = Ball.transform.position - transform.position;
        Vector3 ForceTowards = DirectionToBall.normalized * Speed * moveVertical;
        ForceTowards.y = 0;
        rb.AddForce(ForceTowards);

        // Horizontal
        float moveHorizontal = Input.GetAxis("Horizontal");
        if (!isOnTheGround)
            moveHorizontal = moveHorizontal / AirMovementSlowdown;
        Vector3 ForceSide = Vector3.Cross(DirectionToBall.normalized, Vector3.up).normalized * Speed * -moveHorizontal;
        ForceSide.y = 0;
        rb.AddForce(ForceSide);

        // Aerial
        if (Input.GetKey(KeyCode.Space))
            rb.AddForce(new Vector3(0, AerialStrength, 0));

        // Jumping
        if (isOnTheGround || true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                rb.AddForce(rb.velocity.normalized * JumpStrength);
                
//                rb.AddForce(new Vector3(0, JumpStrength, 0));
            }
        }

        // Down Force
        RaycastHit hit;
        Physics.Raycast(transform.position, -Vector3.up, out hit, 1.1f);
        if (hit.distance == 0)
        {
            rb.AddForce(Vector3.down * DownForceAir);
            isOnTheGround = false;
            if (Input.GetKeyDown(KeyCode.LeftControl))
                rb.AddForce(Vector3.down * FallForce);
        }
        else
            isOnTheGround = true;

        //else if(!hit.collider.gameObject.CompareTag("Ball"))
        //Debug.Log(isOnTheGround);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, Ball.transform.position);

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2);

        
        //Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 1.1f);
        //if (hit.distance == 0)
        //    isOnTheGround = false;
        //else 
        //    isOnTheGround = true;
        //Debug.Log(isOnTheGround);
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log(collision.collider.gameObject.tag);
        //Debug.Log(collision.collider.gameObject.name);
        
    }
}
