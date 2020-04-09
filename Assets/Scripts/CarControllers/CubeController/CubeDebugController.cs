using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CubeDebugController : MonoBehaviour
{
    private CubeWheel[] _wheelArray;
    private CubeSphereCollider[] _sphereArray;
    
    private void Start()
    {
        _wheelArray = GetComponentsInChildren<CubeWheel>();
        _sphereArray = GetComponentsInChildren<CubeSphereCollider>();

        _isDrawRaycasts = _sphereArray[0].isDrawContactLines;
    }
    
    [Button("@\"Draw All Contact Lines: \" + _isDrawRaycasts", ButtonSizes.Large)]
    void DrawRaycast()
    {
        _isDrawRaycasts = !_isDrawRaycasts;
        foreach (var sphereCollider in _sphereArray)
        {
            sphereCollider.isDrawContactLines = _isDrawRaycasts;
        }
        
    }
    bool _isDrawRaycasts;
}
