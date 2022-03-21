using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLight : MonoBehaviour
{
    public float speed = 1f;
    public float swingSpeed = .2f;
    float time;

    void Update()
    {
        //Rotate around y-axis
        transform.Rotate(Vector3.forward * Time.deltaTime * speed);

        //Swing back and forth
        time = Mathf.PingPong(Time.time, 4);
        if (time <= 2)
        {
            transform.Rotate(Vector3.down * Time.deltaTime * swingSpeed);
        }
        else { transform.Rotate(Vector3.up * Time.deltaTime * swingSpeed); }
    }
}
