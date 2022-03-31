using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnFloorCollision : MonoBehaviour
{
    Rigidbody rb;
    Vector3 objPosition;
    Quaternion objRotation;

    public void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        objPosition = gameObject.transform.position;
        objRotation = gameObject.transform.rotation;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TriggerFloor") { ResetTransform(); }
    }

    void ResetTransform()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.transform.position = objPosition;
        gameObject.transform.rotation = objRotation;
    }
}
