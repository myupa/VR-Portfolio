using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWaves : MonoBehaviour
{
    bool madeAWave = false;
    Vector3 handSpeed;
    Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        //How fast is hand moving
        if (lastPos != transform.position)
        {
            handSpeed = transform.position - lastPos;
            handSpeed /= Time.deltaTime;
            lastPos = transform.position;
        }
        else if (lastPos == transform.position) 
        {
            handSpeed = new Vector3(0f, 0f, 0f);
            handSpeed /= Time.deltaTime;
        }

        //Only make waves if hand is moving above or below certain speeds
        if (!madeAWave && (handSpeed.magnitude > 2.5f && handSpeed.magnitude < 7f)) 
        {
            madeAWave = true;
            StartCoroutine("MakeAWave");
            StartCoroutine("AllowWave");
        }
    }

    IEnumerator MakeAWave()
    {
        //Set multiplier value based on intensity of hand movement
        float mult = 1f;
        if (handSpeed.magnitude < 1f) { mult = handSpeed.magnitude; }
        else if (handSpeed.magnitude >= 1f) { mult = 1f; }

        //Instantiate the wave
        GameObject wave = Instantiate(Resources.Load("WavePrefab") as GameObject);
        wave.transform.position = gameObject.transform.position;
        wave.transform.localScale = new Vector3(.1f * mult, .1f * mult, .1f * mult);

        //Set velocity
        Rigidbody waveRB = wave.GetComponent<Rigidbody>();
        waveRB.velocity = handSpeed;
        waveRB.velocity *= .5f;

        //Makes wave face the direction it moves
        wave.transform.rotation = Quaternion.LookRotation(waveRB.velocity);

        //Makes wave expand in size as it moves
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * (mult * .75f);
            if (wave != null) { 
                wave.transform.localScale = Vector3.Lerp(new Vector3(.1f * mult, .1f * mult, .1f * mult), new Vector3(.5f * mult, .5f * mult, .5f * mult), time);
                }
            yield return null;
        }

        //If wave hasn't collided and destroyed itself on collision, destroy it after it's done expanding
        if (wave != null) { Destroy(wave); }
    }

    IEnumerator AllowWave()
    {
        //Only allow new waves every 1 second
        yield return new WaitForSeconds(.75f);
        madeAWave = false;
    }

}
