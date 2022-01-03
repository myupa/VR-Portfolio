using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingTriggers : MonoBehaviour
{
    public GameObject leftGrabCollider;
    public GameObject leftReleaseCollider;
    public GameObject rightGrabCollider;
    public GameObject rightReleaseCollider;
    bool leftGrabbing = false;
    bool rightGrabbing = false;

    public void OnLeftGrab()
    {
        leftGrabbing = true;
        if (leftReleaseCollider != null) { leftReleaseCollider.SetActive(false); }
        if (rightReleaseCollider != null) { rightReleaseCollider.SetActive(false); }
        if (leftGrabCollider != null) { leftGrabCollider.SetActive(true); }
    }

    public void OnRightGrab()
    {
        rightGrabbing = true;
        if (rightReleaseCollider != null) { rightReleaseCollider.SetActive(false); }
        if (leftReleaseCollider != null) { leftReleaseCollider.SetActive(false); }
        if (rightGrabCollider != null) { rightGrabCollider.SetActive(true); }
    }

    public void OnLeftRelease()
    {
        leftGrabbing = false;
        if (!leftGrabbing && !rightGrabbing)
        {
            if (leftReleaseCollider != null) { leftReleaseCollider.SetActive(true); }
            if (rightReleaseCollider != null) { rightReleaseCollider.SetActive(true); }
        }
        if (leftGrabCollider != null) { leftGrabCollider.SetActive(false); }
    }

    public void OnRightRelease()
    {
        rightGrabbing = false;
        if (!leftGrabbing && !rightGrabbing)
        {
            if (leftReleaseCollider != null) { leftReleaseCollider.SetActive(true); }
            if (rightReleaseCollider != null) { rightReleaseCollider.SetActive(true); }
        }
        if (rightGrabCollider != null) { rightGrabCollider.SetActive(false); }
    }
}
