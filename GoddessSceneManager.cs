using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GoddessSceneManager : SceneManagerParentClass
{
    private static GoddessSceneManager _goddessSM;
    public static GoddessSceneManager goddessSM
    {
        get
        {
            if (_goddessSM == null) UnityEngine.Debug.LogError("Goddess Scene Manager is null");
            return _goddessSM;
        }
    }
    private void Awake() { _goddessSM = this; }

    [Header("Miscellaneous")]
    public GameObject PlayerGO;
    public Material skyboxCube;
    public Animator godAnimator;
    public GameObject StartingText;
    Transform PlayerTransform;
    Quaternion targetrotx;//= new Quaternion();

    [Header("Water & Rain")]
    public GameObject risingWater;
    public Vector3 waterheightStart;
    public Vector3 waterheightEnd;
    public GameObject rScriptGO;

    [Header("Audio")]
    public AudioMixerSnapshot Unmuffled;
    public AudioMixerSnapshot Muffled;

    public AudioClip godClip;
    public AudioSource goddessAS;
    [Range(0, 1)] public float godVolume;

    public AudioClip forestMusicBackground;
    public AudioSource forestmusicS;
    [Range(0, 1)] public float forestMusicVolume;

    [HideInInspector] public bool underwater;
    bool seqActivated = false;

    public static Action FadeInClouds;
    public void FadeInCloudsMethod() { if (FadeInClouds != null) FadeInClouds(); }

    public void Start()
    {
        StartCoroutine("StartingScreenRoutine");
        targetrotx.Set(0f, 0f, 0.9999383f, 0.0111104f);
        print(targetrotx);
        //return targetrotx;
    } 
    
    void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
    }

    IEnumerator StartingScreenRoutine()
    {
        yield return new WaitForSeconds(7f);
        if (StartingText != null) { StartingText.SetActive(true); }
        yield return new WaitForSeconds(1f);
        if (pressAnyButton != null)
        {
            pressAnyButton.action.Enable();
            pressAnyButton.action.performed += context => StartSequenceMethod();
        }
    }

    public void StartSequenceMethod()
    {
        if (!seqActivated) { seqActivated = true; StartCoroutine("StartSequences"); }
        if (pressAnyButton != null)
        {
            pressAnyButton.action.Disable();
            pressAnyButton.action.performed -= context => StartSequenceMethod();
        }
    }

    IEnumerator StartSequences()
    {
        SceneTransitionManager.STManager.FadeOutPrompt(4f);
        if (StartingText != null) { StartingText.SetActive(false); }
        if (forestMusicBackground != null) { forestmusicS.PlayOneShot(forestMusicBackground, forestMusicVolume); }

        yield return new WaitForSeconds(13f); ;

        if (goddessAS != null) { goddessAS.Play(); }
        if (godAnimator != null) { godAnimator.SetTrigger("godAnimTrigger"); }

        yield return new WaitForSeconds(19.5f);

        StartCoroutine(WaterRising());
        StartCoroutine(RainDown());
        FadeInClouds();
        StartCoroutine(DarkenSkybox());
        PlayerTransform = PlayerGO.GetComponent<Transform>();
        StartCoroutine(FallThroughFloor());
        StartCoroutine(FallBackwards());

        yield return new WaitForSeconds(14f);

        SceneTransitionManager.STManager.GoToScene("Loading Brain", 1f);
    }

    /*
    public IEnumerator FogFade()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * .15f;
            RenderSettings.fogDensity = Mathf.Lerp(.25f, .0724f, Mathf.SmoothStep(0.0f, 1.0f, time));
            yield return null;
        }
    }
    */

    public IEnumerator RainDown()
    {
        if (rScriptGO != null)
        {
            yield return new WaitForSeconds(2.5f);
            DigitalRuby.RainMaker.RainScript rScript = rScriptGO.GetComponent<DigitalRuby.RainMaker.RainScript>();
            float time = 0f;
            while (time < 1f)
            {
                //float rFLOAT = rScript.GetComponent<RainIntensity>();//.RainIntensity;
                time += Time.deltaTime * .1f;
                rScript.RainIntensity = Mathf.Lerp(0, 1, time);
                yield return null;
            }
        }
    }

    public IEnumerator WaterRising()
    {
        if (risingWater != null)
        {
            yield return new WaitForSeconds(4.5f);
            // start = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            // des = new Vector3(transform.position.x, 10f, transform.position.z);
            risingWater.SetActive(true);
            Transform WaterTransform = risingWater.GetComponent<Transform>();
            float time = 0f;

            while (time < 10)
            {
                time += Time.deltaTime * .05f;
                WaterTransform.transform.position = Vector3.Lerp(waterheightStart, waterheightEnd, time);
                yield return null;
            }
        }
    }

    public IEnumerator DarkenSkybox()
    {
        yield return new WaitForSeconds(2f);

        float time = 0f;
        while (time < 8f)
        {
            //yield return new WaitForSeconds(2f);
            time += Time.deltaTime * .1f;
            float ExposureF = Mathf.Lerp(1f, .5f, time);
            RenderSettings.skybox.SetFloat("_Exposure", ExposureF);
            yield return null;
        }
    }

    public IEnumerator FallThroughFloor()
    {
        if (PlayerGO != null)
        {
            yield return new WaitForSeconds(13.5f);
            float playerfallLerp = 0f;

            Vector3 initialposy = PlayerTransform.position;
            Vector3 targetposy = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y - 2f, PlayerTransform.position.z);

            while (playerfallLerp < 1f)
            {
                playerfallLerp += Time.deltaTime * .5f;
                PlayerTransform.position = Vector3.Lerp(initialposy, targetposy, playerfallLerp); 
                yield return null;
            }
        }
    }

    public IEnumerator FallBackwards()
    {
        if (PlayerGO != null)
        {
            yield return new WaitForSeconds(14f);
            float playerLerp = 0f;

            Quaternion initialrotx = PlayerTransform.rotation;
            //Quaternion targetrotx = Quaternion.Euler(initialrotx.x - 70f, initialrotx.y - 120f, initialrotx.z - 0f);

            while (playerLerp < 2.5f)
            {
                playerLerp += Time.deltaTime * .2f;
                PlayerTransform.rotation = Quaternion.Lerp(initialrotx, targetrotx, playerLerp);
                yield return null;
            }
        }
    }

    public override void SetZone(Zone zone)
    {
        switch (zone)
        {
            case Zone.Underwater:
                if (Muffled != null) { Muffled.TransitionTo(0f); }
                break;
            case Zone.Abovewater:
                if (Unmuffled != null) { Unmuffled.TransitionTo(0f); }
                break;
            default:
                break;
        }
    }
}
