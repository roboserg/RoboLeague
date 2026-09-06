using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParticleSystem : MonoBehaviour
{
    public ParticleSystem boostPs;
    public GameObject firePs;

    const int SupersonicThreshold = 2200 / 100;
    //CubeController _controller;
    private SimpleAirControl _airControl;
    //private TrailRenderer[] _trails;
    bool _isBoostAnimationPlaying = false;

    void Start()
    {
        //_controller = GetComponentInParent<CubeController>();
        //_trails = GetComponentsInChildren<TrailRenderer>();
        _airControl = GetComponentInParent<SimpleAirControl>();

        firePs.SetActive(false);
    }

    void Update()
    {
        if (_airControl.inputBoost)
        {
            if (_isBoostAnimationPlaying == false)
            {
                boostPs.Play();
                firePs.SetActive(true);
                _isBoostAnimationPlaying = true;
            }
        }
        else if (!_airControl.inputBoost)
        {
            boostPs.Stop();
            firePs.SetActive(false);
            _isBoostAnimationPlaying = false;
        }
    }

    const float TrailLength = 0.075f;
}
