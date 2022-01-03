using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopGravity : MonoBehaviour
{
    Rigidbody rb;
    Collider coll;

    void Start()
    {
        PrepareForNoGravity();
        MushroomSceneManager.FloatRocksUpwards += TurnOffGravity;
    }

    void PrepareForNoGravity()//runb this off separate event/delegate tihng
    {
        if (rb == null)
        {
            if (gameObject.GetComponent<Rigidbody>() == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = true;
            }
            if (gameObject.GetComponent<Rigidbody>() != null)
            {
                rb = gameObject.GetComponent<Rigidbody>();
                if (!rb.useGravity) { rb.useGravity = true; }
                if (!rb.isKinematic) { rb.isKinematic = true; }
            }
        }
        if (coll == null)
        {
            if (gameObject.GetComponent<Collider>() == null)
            {
                coll = gameObject.AddComponent<SphereCollider>() as SphereCollider;
            }
            if (gameObject.GetComponent<Collider>() != null)
            {
                coll = gameObject.GetComponent<Collider>();
            }
        }

    }

    void TurnOffGravity()
    {
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.AddForce(0, 1, 0, ForceMode.Impulse);
    }

}
