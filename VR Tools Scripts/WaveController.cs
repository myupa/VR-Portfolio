using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public Rigidbody rb;

    IEnumerator OnCollisionEnter()
    {
        yield return new WaitForSeconds(.1f);
        Destroy(this.gameObject);
    }

}

