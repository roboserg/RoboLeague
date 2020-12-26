using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SimpleAgent : Agent
{
    Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 9, 0);
        transform.rotation = Quaternion.Euler(0,0,0);
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(_rb.velocity);
        sensor.AddObservation(_rb.angularVelocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var inputPitch = actionBuffers.ContinuousActions[0];
        var inputYaw = -actionBuffers.ContinuousActions[1];
        var inputRoll = -actionBuffers.ContinuousActions[2];
        var inputBoost = Mathf.Clamp(actionBuffers.ContinuousActions[3], 0, 1);
        
        // pitch
        _rb.AddTorque(Tp * inputPitch * transform.right, ForceMode.Acceleration);
        _rb.AddTorque(transform.right * (Dp * (1 - Mathf.Abs(inputPitch)) * transform.InverseTransformDirection(_rb.angularVelocity).x), ForceMode.Acceleration);
        //yaw
        _rb.AddTorque(Tr * inputYaw * transform.forward, ForceMode.Acceleration);
        _rb.AddTorque(Dr * transform.InverseTransformDirection(_rb.angularVelocity).z * transform.forward, ForceMode.Acceleration);
        // roll
        _rb.AddTorque(Ty * inputRoll * transform.up, ForceMode.Acceleration);
        _rb.AddTorque(transform.up * (Dy * (1 - Mathf.Abs(inputRoll)) * transform.InverseTransformDirection(_rb.angularVelocity).y), ForceMode.Acceleration);
        //boost
        _rb.AddForce(transform.up * (9.91f * inputBoost * 1.5f), ForceMode.Acceleration);
        
        if(Mathf.Abs(transform.localPosition.y) > 30 || Mathf.Abs(transform.localPosition.x) > 45 || Mathf.Abs(transform.localPosition.z) > 45)
            EndEpisode();
        else
            AddReward(0.01f);
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ground"))
            EndEpisode();
    }
    
    #region Torque Coefficients for rotation and drag
    const float Tr = 36.07956616966136f; // torque coefficient for roll
    const float Tp = 12.14599781908070f; // torque coefficient for pitch
    const float Ty = 8.91962804287785f; // torque coefficient for yaw
    const float Dr = -4.47166302201591f; // drag coefficient for roll
    const float Dp = -2.798194258050845f; // drag coefficient for pitch
    const float Dy = -1.886491900437232f; // drag coefficient for yaw
    #endregion
}
