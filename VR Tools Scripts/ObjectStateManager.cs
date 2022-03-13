using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStateManager : MonoBehaviour
{
    Vector3 startPosition;
    Quaternion startRotation;

    public void Start()
    {
        VRToolsManager.instance.resetObjectState += ResetThisObject;
        startPosition = gameObject.transform.localPosition;
        startRotation = gameObject.transform.localRotation;
    }

    public void OnDisable()
    {
        VRToolsManager.instance.resetObjectState -= ResetThisObject;
    }

    public void ResetThisObject()
    {
        gameObject.transform.position = startPosition;
        gameObject.transform.rotation = startRotation;
        if (gameObject.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
