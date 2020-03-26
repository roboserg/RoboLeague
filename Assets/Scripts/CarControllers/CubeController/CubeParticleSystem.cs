using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParticleSystem : MonoBehaviour
{
    public ParticleSystem WindPS, BoostPS;
    public GameObject FirePS;

    const int supersonicThreshold = 2200 / 100;
    CubeController controller;
    bool isBoostAnimationPlaying = false;

    void Start()
    {
        controller = GetComponentInParent<CubeController>();
        FirePS.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButton("RB") || Input.GetMouseButton(0))
        {
            if (isBoostAnimationPlaying == false)
            {
                BoostPS.Play();
                FirePS.SetActive(true);
                isBoostAnimationPlaying = true;
            }

        }
        else if (!(Input.GetButton("RB") || Input.GetMouseButton(0)))
        {
            BoostPS.Stop();
            FirePS.SetActive(false);
            isBoostAnimationPlaying = false;
        }

        //  Wind effect
        if (controller.forwardSpeed >= supersonicThreshold)
            WindPS.Play();
        else
            WindPS.Stop();
    }
}
