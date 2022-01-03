using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireUnitsScript : MonoBehaviour
{
    public int assignedInt;
    bool alreadyTriggered = false;
    public ParticleSystem fireParticleSystem;
    public Light fireLight;
    float burningLength;
    float timeDivider;
    public AudioSource fireAS1;
    public AudioSource fireAS2;
    public bool isHeadMushroom;
    float isHeadMushroomMultiplier = 1f;

    void Start()
    {
        burningLength = Random.Range(11f, 15f);
        if (isHeadMushroom) { isHeadMushroomMultiplier = .6f; } //Make the head mushroom burn longer
        timeDivider = ( 1 / burningLength ) * isHeadMushroomMultiplier;
        //if (assignedInt == 4) { Invoke("ExternalSetFire", 5f); }
    }

    void OnTriggerEnter(Collider other) //This mushroom head is lit on fire by torch
    {
        if (!alreadyTriggered && other.tag == "FireTrigger")
        {
            alreadyTriggered = true;
            fireLight.enabled = true;
            if (fireAS1 != null) { fireAS1.Play(); }
            if (fireAS2 != null) { fireAS2.Play(); }
            MushroomSceneManager.SMInstance.Invoke("SequenceThree", 0f);
            StartCoroutine("LerpParticlesAmount");
            StartCoroutine("LerpParticlesSpeed");
            StartCoroutine("SpreadFire");
            StartCoroutine("FadeOutLights");
            var emission = fireParticleSystem.emission;
            emission.enabled = true;
            StartCoroutine("FadeOutAudio");
        }
    }

    public void ExternalSetFire()//This mushroom head was set on fire by another mushroom head
    {
        if (!alreadyTriggered)
        {
            alreadyTriggered = true;
            fireLight.enabled = true;
            if (fireAS1 != null) { fireAS1.Play(); }
            if (fireAS2 != null) { fireAS2.Play(); }
            var emission = fireParticleSystem.emission;
            emission.enabled = true;
            StartCoroutine("LerpParticlesAmount");
            StartCoroutine("LerpParticlesSpeed");
            StartCoroutine("SpreadFire");
            StartCoroutine("FadeOutLights");
            StartCoroutine("FadeOutAudio");
        }
    }

    IEnumerator SpreadFire() //Spread fire from mushroom head to mushroom head
    {
        int burnNextDown = assignedInt - 1;
        int burnNextUp = assignedInt + 1;

        yield return new WaitForSeconds(1.6f);
        if (burnNextDown >= 0) { MushroomSceneManager.SMInstance.fireTriggerScripts[burnNextDown].ExternalSetFire(); }

        yield return new WaitForSeconds(2.2f);
        if (burnNextUp < 7) {  MushroomSceneManager.SMInstance.fireTriggerScripts[burnNextUp].ExternalSetFire(); }
    }

    public IEnumerator LerpParticlesSpeed() // Lerp down the speed of the flames
    {
        var emission = fireParticleSystem.emission;
        var main = fireParticleSystem.main;
        float initialSimulationSpeed = main.simulationSpeed;
        float finalSpeed = initialSimulationSpeed *.15f;
        yield return new WaitForSeconds(3f);

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * timeDivider;
            main.simulationSpeed = Mathf.Lerp(initialSimulationSpeed, finalSpeed, time);
            yield return null;
        }
        emission.enabled = false;
    }

    public IEnumerator LerpParticlesAmount() // Lerp down the # of particles in each fire
    {
        var main = fireParticleSystem.main;
        float initialMaxParticles = main.maxParticles;
        yield return new WaitForSeconds(3f);

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * timeDivider;
            main.maxParticles = (int)Mathf.Lerp(initialMaxParticles, 5f, time);
            yield return null;
        }
    }

    public IEnumerator FadeOutLights()//Fade out lights in the flames
    {
        float time = 0;
        float smallerTimeDivider = timeDivider * .005f;
        yield return new WaitForSeconds(7f);

        while (time < 1)
        {
            time += Time.deltaTime * smallerTimeDivider;
            float initialIntensity = fireLight.intensity;
            fireLight.intensity = Mathf.Lerp(initialIntensity, 0f, time);
            yield return null;
        }
    }

    public IEnumerator FadeOutAudio() //Fade out fire sounds
    {
        if (fireAS1 != null)
        {
            float time = 0;
            float initialVolume = fireAS1.volume;
            float audioTimeDivider = timeDivider;
            while (time < 1)
            {
                time += Time.deltaTime * audioTimeDivider;
                fireAS1.volume = Mathf.Lerp(initialVolume, 0f, time);
                yield return null;
            }
            fireAS1.Stop();
        }
    }
}
