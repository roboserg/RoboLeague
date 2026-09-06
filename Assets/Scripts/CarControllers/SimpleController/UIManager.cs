using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    SimpleController _controller;
    public Agent _agent; 
    GUIStyle _style;
    
    void Start()
    {
        _controller = _agent.GetComponentInChildren<SimpleController>();
        
        // GUI stuff
        _style = new GUIStyle();
        _style.normal.textColor = Color.red;
        _style.fontSize = 25;
        _style.fontStyle = FontStyle.Bold;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnGUI()
    {
        GUI.Label(new Rect(10.0f, 10.0f, 150, 130), $"Vel Forward: {_controller.velForward:F2} m/s \t Vel Magnitude: {_controller.velMagn:F0} m/s", _style);
        GUI.Label(new Rect(10.0f, 40.0f, 150, 130), $"Total Reward: {_agent.GetCumulativeReward():F2} \t Step Count: {_agent.StepCount:F0}", _style);
        //GUI.Label(new Rect(30.0f, 60.0f, 150, 130), $"car state: {carState.ToString()}", _style);
    }
}
