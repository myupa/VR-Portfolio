using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedForSpeed : MonoBehaviour
{
    [Range(0f, 5f)] public float newTimeScale = 1f;
    protected float fixedDeltaTime;

    void Start()
    {
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
       Time.timeScale = newTimeScale;
       Time.fixedDeltaTime = this.fixedDeltaTime * newTimeScale;
    }
}
