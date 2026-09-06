using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBoxTrigger : MonoBehaviour
{
    //public bool isTouchingSurface = false;
    public bool isDebug = false;
    CubeController controller;
    Collider col;
    void Start()
    {
        controller = GetComponentInParent<CubeController>();
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Map"))
        {
            controller.isBodySurface = true;
            //isTouchingSurface = true;
        }
        if (isDebug)
            print(transform.name + " is in contact with " + other.tag);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Map"))
        {
            controller.isBodySurface = false;
            //isTouchingSurface = false;
        }
        if (isDebug)
            print(transform.name + " is no longer in contact with " + other.tag);
    }
}
