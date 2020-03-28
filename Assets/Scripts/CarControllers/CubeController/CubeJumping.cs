using UnityEngine;

public class CubeJumping : MonoBehaviour
{
    [Header("Forces")]
    [Range(1,4)]
    public float jumpForceMultiplier = 1;
    public int upForce = 3;
    public int upTorque = 50;

    float jumpTimer = 0;
    bool _isMouseButton1 = false;
    bool _isMouseButtonDown1 = false;
    bool _isMouseButtonUp1 = false;

    bool isCanFirstJump = false;
    bool isJumping = false;
    bool isCanKeepJumping = false;

    Rigidbody _rb;
    CubeController _controller;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponent<CubeController>();
    }

    void Update()
    {
        if (Input.GetMouseButton(1) || Input.GetButton("A"))
            _isMouseButton1 = true;

        if (Input.GetMouseButtonDown(1) || Input.GetButtonDown("A"))
            _isMouseButtonDown1 = true;

        if (Input.GetMouseButtonUp(1) || Input.GetButtonUp("A"))
            _isMouseButtonUp1 = true;

        JumpBackToTheFeet();
    }

    private void FixedUpdate()
    {
        // Reset jump flags when landed
        if (_controller.isAllWheelsSurface)
        {
            // Need a timer, otherwise while jumping we are setting isJumping flag to false right on the next frame 
            if (jumpTimer >= 0.1f)
                isJumping = false;

            jumpTimer = 0;
            isCanFirstJump = true;
            //if (isDebug)    controller.ClearConsole();
        }
        // Cant start jumping while in the air
        else if (!_controller.isAllWheelsSurface)
            isCanFirstJump = false;

        // Do initial jump impulse only once
        if (_isMouseButtonDown1 && isCanFirstJump)
        {
            isCanKeepJumping = true;
            isCanFirstJump = false;
            isJumping = true;
            _rb.AddForce(transform.up * 291.667f / 100 * jumpForceMultiplier, ForceMode.VelocityChange);

            jumpTimer += Time.fixedDeltaTime;
        }

        // If jump button was released we can't start jumping again mid air
        if(_isMouseButtonUp1)
            isCanKeepJumping = false;

        // Keep jumping if jump button pressed
        if (_isMouseButton1 && isJumping && isCanKeepJumping && jumpTimer <= 0.2f )
        {
            _rb.AddForce(transform.up * 1458f / 100 * jumpForceMultiplier, ForceMode.Acceleration);
            jumpTimer += Time.fixedDeltaTime;
        }

        _isMouseButton1 = false;
        _isMouseButtonDown1 = false;
        _isMouseButtonUp1 = false;
    }
    
    //Auto jump and rotate when the car is on the roof
    void JumpBackToTheFeet()
    {
        if (_controller.carState != CubeController.CarStates.BodyGroundDead) return;
        
        if (_isMouseButtonDown1 || Input.GetButtonDown("A"))
        {
            _rb.AddForce(Vector3.up * upForce, ForceMode.VelocityChange);
            _rb.AddTorque(transform.forward * upTorque, ForceMode.VelocityChange);
        }
    }
}