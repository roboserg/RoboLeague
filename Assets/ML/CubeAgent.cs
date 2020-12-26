using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CubeAgent : Agent
{
    Rigidbody _rBody;
    CubeController _controller;
    void Start ()
    {
        _rBody = GetComponent<Rigidbody>();
        _controller = GetComponent<CubeController>();
        _controller.carState = CubeController.CarStates.Air;
    }

    public override void OnEpisodeBegin()
    {
        _controller.carState = CubeController.CarStates.Air;
        _controller.transform.position = new Vector3(0, 9, 0);
        _controller.transform.rotation = Quaternion.Euler(-90,0,0);
        _rBody.velocity = Vector3.zero;
        _rBody.angularVelocity = Vector3.zero;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_controller.transform.position);
        sensor.AddObservation(_controller.transform.rotation);
        sensor.AddObservation(_rBody.velocity);
        sensor.AddObservation(_rBody.angularVelocity);
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        GameManager.InputManager.pitchInput = actionBuffers.ContinuousActions[0];
        GameManager.InputManager.yawInput = actionBuffers.ContinuousActions[1];
        GameManager.InputManager.isBoost = actionBuffers.ContinuousActions[2] > 0;
        //GameManager.InputManager.isBoost = actionBuffers.DiscreteActions[0] != 0;

        /*switch (actionBuffers.DiscreteActions[1])
        {
            case 0:
                GameManager.InputManager.rollInput = 0;
                break;
            case 1:
                GameManager.InputManager.rollInput = 1;
                break;
            case 2:
                GameManager.InputManager.rollInput = -1;
                break;
        }*/

        if (_controller.carState != CubeController.CarStates.Air)
        {
            AddReward(-1);
            EndEpisode();
        }
        else
            AddReward(0.1f);
        
        /*switch (throttle)
        {
            case 0:
                GameManager.InputManager.pitchInput = 0;
                break;
            case 1:
                GameManager.InputManager.pitchInput = 1;
                break;
            case 2:
                GameManager.InputManager.pitchInput = -1;
                break;
            default:
                GameManager.InputManager.pitchInput = 0;
                break;
        }
        
        switch (steering)
        {
            case 0:
                GameManager.InputManager.yawInput = 0;
                break;
            case 1:
                GameManager.InputManager.yawInput = 1;
                break;
            case 2:
                GameManager.InputManager.yawInput = -1;
                break;
            default:
                GameManager.InputManager.yawInput = 0;
                break;
        }*/
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : -1;
        /*switch (Mathf.Round(Input.GetAxis("Vertical")))
        {
            case 0:
                continuousActionsOut[0] = 0;
                break;
            case 1:
                continuousActionsOut[0] = 1;
                break;
            case -1:
                continuousActionsOut[0] = 2;
                break;
        }
        switch (Mathf.Round(Input.GetAxis("Horizontal")))
        {
            case 0:
                continuousActionsOut[1] = 0;
                break;
            case 1:
                continuousActionsOut[1] = 1;
                break;
            case -1:
                continuousActionsOut[1] = 2;
                break;
        }*/
    }
}