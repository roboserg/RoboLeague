using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour
{
    public InputMaster controls;

    private void Awake()
    {
        controls = new InputMaster();
        controls.PlayerControls.Jump.performed += _ => Jump();
        controls.PlayerControls.Move.performed += cnt => Move(cnt.ReadValue<Vector2>());
        
    }

    void Jump()
    {
        Debug.Log("We jumped :D");
    }

    void Move(Vector2 dir)
    {
        Debug.Log("We moved " + dir);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
