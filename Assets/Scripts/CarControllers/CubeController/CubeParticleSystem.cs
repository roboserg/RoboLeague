using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParticleSystem : MonoBehaviour
{
    public ParticleSystem windPs, boostPs;
    public GameObject firePs;

    const int SupersonicThreshold = 2200 / 100;
    CubeController _controller;
    private TrailRenderer[] _trails;
    bool _isBoostAnimationPlaying = false;

    void Start()
    {
        _controller = GetComponentInParent<CubeController>();
        _trails = GetComponentsInChildren<TrailRenderer>();
        _trails[0].time = _trails[1].time = 0;
        firePs.SetActive(false);

    }

    void Update()
    {
        if (Input.GetButton("RB") || Input.GetMouseButton(0))
        {
            if (_isBoostAnimationPlaying == false)
            {
                boostPs.Play();
                firePs.SetActive(true);
                _isBoostAnimationPlaying = true;
            }

        }
        else if (!(Input.GetButton("RB") || Input.GetMouseButton(0)))
        {
            boostPs.Stop();
            firePs.SetActive(false);
            _isBoostAnimationPlaying = false;
        }
    }

    const float TrailLength = 0.075f;

    private void FixedUpdate()
    {
        //  Wind and trail effect
        if (_controller.forwardSpeed >= SupersonicThreshold)
        {
            windPs.Play();
            
            if (_controller.isAllWheelsSurface)
                _trails[0].time = _trails[1].time = Mathf.Lerp(_trails[1].time, TrailLength, Time.fixedDeltaTime * 5);
            else 
                _trails[0].time = _trails[1].time = 0;
        }
        
        else
        {
            windPs.Stop();
            
            _trails[0].time = _trails[1].time = Mathf.Lerp(_trails[1].time, 0.029f, Time.fixedDeltaTime * 6);
            if (_trails[0].time <= 0.03f)
                _trails[0].time = _trails[1].time = 0;
        }
    }
}
