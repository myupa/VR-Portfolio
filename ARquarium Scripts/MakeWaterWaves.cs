using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWaterWaves : MonoBehaviour
{
    public AudioManager audioManager;
    [Range(1.0f, 6.0f)] public float Sensitivity; //Minimum velocity
    public bool isRightHand = false;

    bool madeAWave;
    Vector3 handSpeed;
    Vector3 lastPos;

    void Enable()
    {
        madeAWave = false;
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
        if (!madeAWave && (handSpeed.magnitude > Sensitivity && handSpeed.magnitude < 6.1f)) 
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

        //Set length of wave's lifetime
        if (wave.TryGetComponent (out WaveController controller))
        {
            controller.SetLifetime(mult);
        }

        //Set velocity
        Rigidbody waveRB = wave.GetComponent<Rigidbody>();
        waveRB.velocity = handSpeed;
        waveRB.velocity *= .5f;

        //Make wave visuals face the direction it moves
        wave.transform.rotation = Quaternion.LookRotation(waveRB.velocity);

        //Play intensity-specific water audio
        if (audioManager != null) { audioManager.PlayHandWave(mult, isRightHand); }

        yield return null;
    }

    IEnumerator AllowWave()
    {
        //Only allow new waves every .75 second
        yield return new WaitForSeconds(.75f);
        madeAWave = false;
    }

}
