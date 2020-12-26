using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AirDribbleAgent : Agent
{
    public bool isAlwaysRoll = false;
    public Transform ballTransform;
    Rigidbody _rb, _rbBall;
    private SimpleAirControl _airControl;
    private SimpleController _controller;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rbBall = ballTransform.GetComponent<Rigidbody>();
        _airControl = GetComponentInChildren<SimpleAirControl>();
        _controller = GetComponentInChildren<SimpleController>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 7, 0);
        transform.rotation = Quaternion.Euler(-90,0,0);
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        
        ballTransform.localPosition = new Vector3(0, 9, 0);
        _rbBall.velocity = Vector3.zero;
        _rbBall.angularVelocity = Vector3.zero;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(_rb.velocity);
        sensor.AddObservation(_rb.angularVelocity);
        //Ball
        sensor.AddObservation(ballTransform.localPosition);
        sensor.AddObservation(_rbBall.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float inputPitch = actionBuffers.ContinuousActions[0];
        float inputYaw = actionBuffers.ContinuousActions[1];
        float inputRoll = actionBuffers.ContinuousActions[2];
        inputRoll = inputRoll > 0.33f ? 1 : inputRoll;
        inputRoll = inputRoll < -0.33f ? -1 : inputRoll;
        bool inputBoost = actionBuffers.ContinuousActions[3] > 0;

        _airControl.inputYaw = inputYaw;
        _airControl.inputPitch = inputPitch;
        _airControl.inputRoll = inputRoll;
        if(isAlwaysRoll) _airControl.inputRoll = 1;
        _airControl.inputBoost = inputBoost;
        
        //if(Mathf.Abs(transform.localPosition.y) > 50 || Mathf.Abs(transform.localPosition.x) > 60 || Mathf.Abs(transform.localPosition.z) > 60)
        //    EndEpisode();
        
        //if(Mathf.Abs(ballTransform.localPosition.y) > 50 || Mathf.Abs(ballTransform.localPosition.x) > 60 || Mathf.Abs(ballTransform.localPosition.z) > 60)
        //    EndEpisode();
        
        if(ballTransform.localPosition.y < 1.5f)
            EndEpisode();
        
        AddReward(0.01f);
        //AddReward(inputRoll/300); // 0.01
        //AddReward(inputBoost ? 0.01f : 0);
        //Debug.Log(actionBuffers.ContinuousActions[2]);
        //Debug.Log(inputRoll);
        //AddReward(-_controller.velMagn / 1000);
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[3] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        
        continuousActionsOut[2] = 0;
        if (Input.GetKey(KeyCode.E) || Input.GetButton("B"))
            continuousActionsOut[2] = -1;
        else if (Input.GetKey(KeyCode.Q) || Input.GetButton("Y"))
            continuousActionsOut[2] = 1;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //if(other.gameObject.CompareTag("Ground"))
        //    EndEpisode();
    }
}
