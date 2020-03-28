using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CubeDebugController : MonoBehaviour
{
    [Title("Debug Draw Options")]
    [Button("@\"Draw All Wheel Velocities: \" + _isDrawWheelVelocities", ButtonSizes.Large)]
    void IsDrawWheelVelocities()
    {
        var wheelArray = GetComponentsInChildren<CubeWheel>();
        _isDrawWheelVelocities = !_isDrawWheelVelocities;

        foreach (var wheel in wheelArray)
        {
            wheel.isDrawWheelVelocities = !wheel.isDrawWheelVelocities;
        }
    }
    bool _isDrawWheelVelocities = false;
    
    [Button("@\"Draw All Wheel Discs: \" + _isDrawWheelDisc", ButtonSizes.Large)]
    void IsDrawWheelDisc()
    {
        var sphereArray = GetComponentsInChildren<CubeSphereCollider>();
        _isDrawWheelDisc = !_isDrawWheelDisc;

        foreach (var sphere in sphereArray)
        {
            sphere.isDrawWheelDisc = !sphere.isDrawWheelDisc;
        }
    }
    bool _isDrawWheelDisc = false;
    
    [Button("@\"Draw All Raycasts: \" + _isDrawRaycasts", ButtonSizes.Large)]
    void IsDrawRaycast()
    {
        _isDrawRaycasts = !_isDrawRaycasts;
        var sphereArray = GetComponentsInChildren<CubeSphereCollider>();

        foreach (var sphere in sphereArray)
        {
            sphere.isDrawRaycast = !sphere.isDrawRaycast;
        }
    }
    bool _isDrawRaycasts = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
