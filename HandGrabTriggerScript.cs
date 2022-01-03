using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabTriggerScript : MonoBehaviour
{

    Collider thisCollider;

    void Start()
    {
        thisCollider = gameObject.GetComponent<Collider>();
    }
    
    public void TurnOnCollider()
    {
        thisCollider.enabled = true;
    }

    public void TurnOffCollider()
    {
        thisCollider.enabled = false;
    }
}
