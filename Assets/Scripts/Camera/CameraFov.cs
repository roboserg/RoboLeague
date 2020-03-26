using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFov : MonoBehaviour
{
    public float fovSpeed = 4f;
    //public float fovSuperSonic = 
    Camera cam;
    float targetFOV, FOV;
    void Start()
    {
        cam = GetComponent<Camera>();
        FOV = targetFOV = cam.fieldOfView;
    }

    void Update()
    {
        FOV = Mathf.Lerp(FOV, targetFOV, Time.deltaTime * fovSpeed);
        cam.fieldOfView = FOV;
    }

    public void SetCamFOV(float targetFov)
    {
        this.targetFOV = targetFov;
    }
}
