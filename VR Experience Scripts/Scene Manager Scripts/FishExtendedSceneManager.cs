using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class FishExtendedSceneManager : SceneManagerParentClass
{

    private static FishExtendedSceneManager _SMinstance;
    public static FishExtendedSceneManager SMInstance
    {
        get
        {
            if (_SMinstance == null) UnityEngine.Debug.LogError("Scene Manager is null");
            return _SMinstance;
        }
    }

    private void Awake()
    {
        _SMinstance = this;
    }

    public GameObject line1;
    public GameObject line2;
    public bool enableStart = false;
    public Animator fishAnimator;
    public GameObject StartTrigger;
    public AudioMixerSnapshot StartAudio;
    public AudioMixerSnapshot louder;
    public AudioClip musicBackgroundClip;
    public AudioSource musicBackgroundS;
    [Range(0, 1)] public float musicVolume;
    public AudioClip fishSpeechClip;
    public AudioSource fishSpeechS;
    [Range(0, 1)] public float fishSpeechVolume;
    public Light RoomLight;
    public Animator giantHuman;
    public Transform giantHumanTransform;
    public GameObject FishRendererGO;
    public GameObject FishTeethRendererGO;
    public GameObject FishTeethRendererGO2;

    public void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
    }

    public IEnumerator Start()
    {
        RenderSettings.reflectionIntensity = 0f;
        if (StartAudio != null) { StartAudio.TransitionTo(0f); }
        if (musicBackgroundS != null) { musicBackgroundS.PlayOneShot(musicBackgroundClip, musicVolume); }
        yield return new WaitForSeconds(1f);
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
        yield return new WaitForSeconds(4f);
        if (fishSpeechS != null) { fishSpeechS.PlayOneShot(fishSpeechClip, fishSpeechVolume); }

        yield return new WaitForSeconds(11.5f);
        TurnLouder();

        yield return new WaitForSeconds(3f);
        StartCoroutine("LerpRoomLight");
        StartCoroutine("LerpEnvironmentReflections");
        onFadeEdges();

        yield return new WaitForSeconds(12f);
        fishFadeOut();

        giantHuman.SetTrigger("StartAnim");
        yield return new WaitForSeconds(8f);
        StartCoroutine("MoveGiant");
        giantHuman.SetTrigger("ReachOver");
        yield return new WaitForSeconds(2.1f);
        StartCoroutine("MoveGiant2");
        yield return new WaitForSeconds(2.1f);
        StartCoroutine("MoveGiant3");

        yield return new WaitForSeconds(2.5f);
        SceneTransitionManager.STManager.GoToScene("GoBackToSleep", 1.5f);
    }

    public void TurnLouder() { if (louder != null) { louder.TransitionTo(1.5f); } }

    IEnumerator LerpRoomLight()
    {
        float  time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * .15f;
            RoomLight.intensity = Mathf.Lerp(0f, 3.9f, time);
            yield return null;
        }
    }

    IEnumerator LerpEnvironmentReflections()
    {
        float time = 0f;
        float f = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * .15f;
            RenderSettings.reflectionIntensity = Mathf.Lerp(0f, .75f, time);
            yield return null;
        }
    }

    public event Action onFadeEdges;
    public void FadeEdges()
    {
        if (onFadeEdges != null)
        {
            onFadeEdges();
        }
    }

    public event Action fishFadeOut;
    public void FishFadeOut()
    {
        if (fishFadeOut != null)
        {
            fishFadeOut();
        }
    }

    public IEnumerator MoveGiant()
    {
        float time = 0f;
        float f = 0f;
        Vector3 oldV = giantHumanTransform.transform.position;
        Vector3 newV = new Vector3(14.5f, -5.704967f, .09f);

        while (time < 1f)
        {
            time += Time.deltaTime * .5f;
            giantHumanTransform.transform.position = Vector3.Lerp(oldV, newV, time);
            yield return null;
        }
    }

    public IEnumerator MoveGiant2()
    {
        float time = 0f;
        float f = 0f;
        Vector3 oldV = giantHumanTransform.transform.position;
        Vector3 newV = new Vector3(13f, -7.804967f, .09f);

        while (time < 1f)
        {
            time += Time.deltaTime * .5f;
            giantHumanTransform.transform.position = Vector3.Lerp(oldV, newV, time);
            yield return null;
        }
    }

    public IEnumerator MoveGiant3()
    {
        float time = 0f;
        float f = 0f;
        Vector3 oldV = giantHumanTransform.transform.position;
        Vector3 newV = new Vector3(13f, -9.404967f, .09f);

        while (time < 1f)
        {
            time += Time.deltaTime * .5f;
            giantHumanTransform.transform.position = Vector3.Lerp(oldV, newV, time);
            yield return null;
        }
    }

}
