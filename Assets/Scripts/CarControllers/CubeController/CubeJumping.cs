using UnityEngine;

[RequireComponent(typeof(CubeController))]
public class CubeJumping : MonoBehaviour
{
    [Header("Forces")]
    [Range(0.25f,4)]
    // default 1
    public float jumpForceMultiplier = 1f;
    public int upForce = 3;
    public int upTorque = 50;
    
    float _jumpTimer = 0;
    [SerializeField]
    bool _isCanFirstJump = false;
    bool _isJumping = false;
    [SerializeField]
    bool _isCanKeepJumping = false;

    Rigidbody _rb;
    CubeController _controller;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponent<CubeController>();
    }

    private void FixedUpdate()
    {
        Jump();
        JumpBackToTheFeet();
    }

    private void Jump()
    {
        // Do initial jump impulse only once
        // TODO: Currently bugged, should be .isJumpDown for the initial jump impulse.
        // Right now does the whole jump impulse
        if (GameManager.InputManager.isJump && _isCanFirstJump)
        {
            _rb.AddForce(transform.up * 292 / 100 * jumpForceMultiplier, ForceMode.VelocityChange);
            _isCanKeepJumping = true;
            _isCanFirstJump = false;
            _isJumping = true;
            
            _jumpTimer += Time.fixedDeltaTime;
        }
        
        // Keep jumping if the jump button is being pressed
        if (GameManager.InputManager.isJump && _isJumping && _isCanKeepJumping && _jumpTimer <= 0.2f)
        {
            _rb.AddForce(transform.up * 1458f / 100 * jumpForceMultiplier, ForceMode.Acceleration);
            _jumpTimer += Time.fixedDeltaTime;
        }
        
        // If jump button was released we can't start jumping again mid air
        if (GameManager.InputManager.isJumpUp)
            _isCanKeepJumping = false;
        
        // Reset jump flags when landed
        if (_controller.isAllWheelsSurface)
        {
            // Need a timer, otherwise while jumping we are setting isJumping flag to false right on the next frame 
            if (_jumpTimer >= 0.1f)
                _isJumping = false;

            _jumpTimer = 0;
            _isCanFirstJump = true;
        }
        // Cant start jumping while in the air
        else if (!_controller.isAllWheelsSurface)
            _isCanFirstJump = false;
    }

    //Auto jump and rotate when the car is on the roof
    void JumpBackToTheFeet()
    {
        if (_controller.carState != CubeController.CarStates.BodyGroundDead) return;
        
        if (GameManager.InputManager.isJumpDown || Input.GetButtonDown("A"))
        {
            _rb.AddForce(Vector3.up * upForce, ForceMode.VelocityChange);
            _rb.AddTorque(transform.forward * upTorque, ForceMode.VelocityChange);
        }
    }
}