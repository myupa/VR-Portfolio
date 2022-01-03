using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishSceneManager : SceneManagerParentClass
{
    public GameObject line1;
    public GameObject line2;
    public bool enableStart = false;
    public Animator fishAnimator;
    public Animator teethAnimator;
    public GameObject StartTrigger;
    public AudioMixerSnapshot louder;
    public AudioClip musicBackgroundClip;
    public AudioSource musicBackgroundS;
    [Range(0, 1)] public float musicVolume;
    public AudioClip fishSpeechClip;
    public AudioSource fishSpeechS;
    [Range(0, 1)] public float fishSpeechVolume;


    public void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
    }

    public IEnumerator Start()
    {
        if (musicBackgroundS != null) { musicBackgroundS.PlayOneShot(musicBackgroundClip, musicVolume); }
        yield return new WaitForSeconds(10f);
        if (StartTrigger != null) { StartTrigger.SetActive(true); }
        else { StartCoroutine("SequenceRoutine"); }
    }

    public void StartSequence()
    {
        StartCoroutine("SequenceRoutine");
    }

    IEnumerator SequenceRoutine()
    {
        if (fishAnimator != null) { fishAnimator.SetTrigger("animTrigger"); }
        if (teethAnimator != null) { teethAnimator.SetTrigger("animTrigger"); }
        yield return new WaitForSeconds(4f);
        if (fishSpeechS != null) { fishSpeechS.PlayOneShot(fishSpeechClip, fishSpeechVolume); }
        yield return new WaitForSeconds(11.5f);
        TurnLouder();
        yield return new WaitForSeconds(7f);
        SceneTransitionManager.STManager.GoToScene("GoBackToSleep", .2f);
    }

    public void TurnLouder() { if (louder != null) { louder.TransitionTo(6f); } }

}
