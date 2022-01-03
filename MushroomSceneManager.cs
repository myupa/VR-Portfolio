using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MushroomSceneManager : SceneManagerParentClass
{

    private static MushroomSceneManager _SMinstance;
    public static MushroomSceneManager SMInstance
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
        AssignValues();
    }

    [Header("Interactables")]
    public BNG.Grabbable torchGrabbable;
    public MeshRenderer TorchRenderer;
    public GameObject SequenceTwoGO;
    public TurnOffKinematic resetOnFloorCollisionScript;
    public GameObject noExitCollider;
    public GameObject darkExit;

    [Header("Screen Overlays")]
    public GameObject StartingText;
    public GameObject EndingText;
    public GameObject line1;
    public GameObject line2;

    [Header("Particle Effects")]
    public ParticleSystem[] wallTorches;
    public ParticleSystem flame;

    [Header("Animations")]
    public FireUnitsScript[] fireTriggerScripts;
    public ShadowController[] shadowsControllers;
    public Animator mushroomAnim;
    public GameObject shadowsParent;

    [Header("Lighting")]
    public Light[] SceneLightsArray;
    public Light[] FlameLights;
    public Light moonLight;
    public Light guidingLight;
    public Light mushroomSpotlight;
    public LightFlickerEffect flameFlickerScript;

    [Header("Audio")]
    public AudioMixerSnapshot MainForestSnapshot;
    public AudioMixerSnapshot MainCaveSnapshot;
    public AudioMixerSnapshot NoFireSnapshot;
    public AudioMixerSnapshot EndingSnapshot;
    public AudioSource mushroomAS;
    public AudioSource fireAS;
    public AudioSource fireAS2;
    public AudioSource Heartbeat;
    public AudioClip MushroomAudioLine1;
    public AudioClip MushroomAudioLine2;

    bool seqThreeActivated = false;
    bool seqOneActivated = false;
    bool lightFinished = false;
    bool leftWorld = false;
    [HideInInspector] public Zone location = Zone.Forest;

    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    public static Action RendererFade;
    public void FadeRendererMethod() { if (RendererFade != null) RendererFade(); }

    public static Action StartNavAgents;
    public void StartWalkingMethod() { if (StartNavAgents != null) StartNavAgents(); }

    public static Action ShadowsSpeak;
    public void ShadowsSpeakMethod() { if (ShadowsSpeak != null) ShadowsSpeak(); }

    public static Action ShadowsGoQuiet;
    public void ShadowsGoQuietMethod() { if (ShadowsGoQuiet != null) ShadowsGoQuiet(); }

    public static Action StopNavAgents;
    public void StopWalkingMethod() { if (StopNavAgents != null) StopNavAgents(); }

    public static Action SpeedUpNavAgents;
    public void SpeedUpNavAgentsMethod() { if (SpeedUpNavAgents != null) SpeedUpNavAgents(); }

    public static Action FloatRocksUpwards;
    public void FloatRocksUpwardsMethod() { if (FloatRocksUpwards != null) FloatRocksUpwards(); }

    public static Action KillOffShadows;
    public void KillOffShadowsMethod() { if (KillOffShadows != null) KillOffShadows(); }

    public void Start() 
    { 
        StartCoroutine("StartingScreenRoutine");
        //StartSequenceOne();
    }

    public void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
        if (Input.GetKey("g")) { StartSequenceOne(); }
    }

    IEnumerator StartingScreenRoutine()
    {
        //StartSequenceOne(); yield return null;
        RenderSettings.skybox.SetFloat("_Exposure", 1.1f);
        yield return new WaitForSeconds(3f);
        if (StartingText != null) { StartingText.SetActive(true); }
        yield return new WaitForSeconds(1f);
        if (pressAnyButton != null)
        {
            pressAnyButton.action.Enable();
            pressAnyButton.action.performed += context => StartSequenceOne();
        }
    }

    public void AssignValues()
    {
        //Numbers the mushroom heads -- this is part of the logic for the fire spreading
        foreach (FireUnitsScript unit in fireTriggerScripts) { unit.assignedInt = System.Array.IndexOf(fireTriggerScripts, unit); }
    }

    public void StartSequenceOne()
    {
        if (!seqOneActivated)
        {
            seqOneActivated = true;
            if (pressAnyButton != null)
            {
                pressAnyButton.action.Disable();
                pressAnyButton.action.performed -= context => StartSequenceOne();
            }
            if (StartingText != null) { StartingText.SetActive(false); }
            SceneTransitionManager.STManager.FadeOutPrompt(4f);

            StartCoroutine("SequenceOneRoutine");
        }
    }

    public IEnumerator SequenceOneRoutine()
    {//Player starts outside. This all starts automatically.Shadows walk past player into cave.
     //After a little while, moonlight flashes and fades out as guiding light shines on cave entrance.
        if (MainForestSnapshot != null) { MainForestSnapshot.TransitionTo(4f); }
        yield return new WaitForSeconds(10f);//switch to 10f later
        ShadowsSpeak();
        yield return new WaitForSeconds(2f);
        StartNavAgents(); 
        yield return new WaitForSeconds(15f);
        SpeedUpNavAgents();
        yield return new WaitForSeconds(28f);
        SequenceTwoGO.SetActive(true);
        yield return new WaitForSeconds(27f);
        StartCoroutine("MoonExposureLerp");

        if (location == Zone.Forest)
        {
            moonLight.intensity = 11f;
            float moonUnFlashTime = 0f;
            while (moonUnFlashTime < 1f)
            {
                moonUnFlashTime += Time.deltaTime * .35f;
                moonLight.intensity = Mathf.Lerp(10f, 3.74f, moonUnFlashTime);
                yield return null;
            }
        }
        else { yield return new WaitForSeconds(3f); }

        StartCoroutine("LerpOffMoonlight");
        yield return new WaitForSeconds(7f);
        StartCoroutine("LerpOnGuidingLight");
    }

    public IEnumerator MoonExposureLerp()
    {
        float moonexposure;
        float moonUnFlashTime = 0f;

        RenderSettings.skybox.SetFloat("_Exposure", 2.5f);

        while (moonUnFlashTime < 1f)
        {
            moonUnFlashTime += Time.deltaTime * .055f;
            moonexposure = Mathf.Lerp(2f, 0f, moonUnFlashTime);
            RenderSettings.skybox.SetFloat("_Exposure", moonexposure);
            yield return null;
        }
    }

    public IEnumerator LerpOnGuidingLight()
    {
        float guidingLerpTime = 0f;
        while (guidingLerpTime < 1f)
        {
            guidingLerpTime += Time.deltaTime * .075f;
            guidingLight.intensity = Mathf.Lerp(0f, 3.74f, guidingLerpTime);
            yield return null;
        }
    }

    public IEnumerator LerpOffMoonlight()
    {
        float moonlerpTime = 0f;
        while (moonlerpTime < 4f)
        {
            moonlerpTime += Time.deltaTime * .05f;
            moonLight.intensity = Mathf.Lerp(3.74f, 0f, moonlerpTime);
            if (moonLight.intensity == 1.25) { moonLight.intensity = 0f; yield break; }
            else if (moonLight.intensity > 1.25f) yield return null;
        }
    }

    public void SequenceTwo()
    {
        StartCoroutine("SequenceTwoRoutine");
    }

    void ShuffleArrayOrder(ShadowController[] array)
    {
        //Rearranges the array of Shadow characters.
        //Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < array.Length; t++)
        {
            ShadowController shadow = array[t];
            int r = UnityEngine.Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = shadow;
        }
    }

    IEnumerator ShadowsDieOff()
    {
        foreach (ShadowController shadow in shadowsControllers)
        {
            if (shadow != null)
            {
                shadow.ExternalKill();
                float wait = UnityEngine.Random.Range(.25f, 1.25f);
                yield return new WaitForSeconds(wait);
            }
        }
    }

    private IEnumerator SequenceTwoRoutine()
    {//Player talks with the Mushroom.
        ShuffleArrayOrder(shadowsControllers);
        yield return new WaitForSeconds(1f);
        StartCoroutine("ShadowsDieOff");
        //KillOffShadows();
        noExitCollider.SetActive(true);
        yield return new WaitForSeconds(8f);
        RendererFade();//Shadows' renderer

        StartCoroutine("ShadowsThroughFloor");
        yield return new WaitForSeconds(10f);
        shadowsParent.SetActive(false);

        StartCoroutine("PlayerScript");
        yield return null;
    }

    public IEnumerator ShadowsThroughFloor()
    {//Dead shadows sink into floor
        Vector3 startPosition = new Vector3(0f, 0f, 0f);
        Vector3 endPosition = new Vector3(0f, -1.6f, 0f);
        yield return new WaitForSeconds(2.0f);
        float fallTime = 0f;
        while (fallTime < 1f)
        {
            fallTime += Time.deltaTime * .05f;
            shadowsParent.transform.position = Vector3.Lerp(startPosition, endPosition, fallTime);
            yield return null;
        }
    }

    public IEnumerator PlayerScript()
    {//The dialogue between mushroom and player
        mushroomAnim.SetTrigger("StartMushroom");
        Heartbeat.Play();
        yield return new WaitForSeconds(3f);

        line1.SetActive(true); yield return new WaitForSeconds(4f);
        PlayAudioClip(mushroomAS, MushroomAudioLine1); FadeUpHeartbeat();  yield return new WaitForSeconds(1f);
        line1.SetActive(false); yield return new WaitForSeconds(23f);
        line2.SetActive(true); yield return new WaitForSeconds(5f); line2.SetActive(false);
        torchGrabbable.enabled = true; StartCoroutine("FadeInTorch"); FlameGrow();
        yield return new WaitForSeconds(2f);
    }

    void FadeUpHeartbeat()
    {
        if (Heartbeat != null) {StartCoroutine("HeartbeatLoudensRoutine"); }
    }

    IEnumerator HeartbeatLoudensRoutine()
    {
        float startVol = Heartbeat.volume;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime *.05f;
            Heartbeat.volume = Mathf.Lerp(startVol, 1f, time);
            yield return null;
        }
    }

    public IEnumerator FadeInTorch()
    {
        Material TorchMaterial = TorchRenderer.GetComponent<MeshRenderer>().material;
        var tempColor = TorchMaterial.color; print("yay?");

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            tempColor.a = Mathf.Lerp(.65f, 1f, time);
            TorchMaterial.color = tempColor;
            yield return null;
        }

        TorchMaterial.SetOverrideTag("RenderType", "");
        TorchMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        TorchMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        TorchMaterial.SetInt("_ZWrite", 1);
        TorchMaterial.DisableKeyword("_ALPHATEST_ON");
        TorchMaterial.DisableKeyword("_ALPHABLEND_ON");
        TorchMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        TorchMaterial.renderQueue = -1;
    }

    public void FlameGrow()
    {//Increase torch flame size
        fireAS2.Play();
        fireAS.volume = 1f;

        var main = flame.main;
        main.startSize = .05f;
        main.maxParticles = 30;
        var emission = flame.emission;
        emission.rateOverTime = 160;
        var shape = flame.shape;
        shape.scale = new Vector3(9f, 11f, 9f);
        velocityModule = flame.velocityOverLifetime;
        velocityModule.yMultiplier = 1.5f;

        if (flameFlickerScript != null) { flameFlickerScript.minIntensity = 1f; flameFlickerScript.maxIntensity = 1.5f; }
    }

    public void SequenceThree()
    {//The mushroom is set on fire. Activated by whichever FireUnitsScript is triggered first
        if (!seqThreeActivated)
        {
            seqThreeActivated = true;
            if (resetOnFloorCollisionScript) { Destroy(resetOnFloorCollisionScript); }
            mushroomAnim.SetTrigger("MushroomDeath");
            PlayAudioClip(mushroomAS, MushroomAudioLine2);
            StartCoroutine("SequenceThreeRoutine");
            StartCoroutine("FadeOutLights");
            StartCoroutine("FadeInMushroomSpotlight");
        }
    }

    public IEnumerator SequenceThreeRoutine()
    {//Mushroom dies as it burns and scene draws to a close.
        //FloatRocksUpwards(); //This effect might be lost with everything else going on
        if (NoFireSnapshot != null) { NoFireSnapshot.TransitionTo(1f); }
        yield return new WaitForSeconds(28f);
        if (EndingSnapshot != null) { EndingSnapshot.TransitionTo(15f); }
        if (darkExit != null) { darkExit.SetActive(true); }
        yield return new WaitForSeconds(21f);
        if (EndingText != null) { EndingText.SetActive(true); }
        yield return new WaitForSeconds(2f);

        if (pressAnyButton != null)
        {
            pressAnyButton.action.Enable();
            pressAnyButton.action.performed += context => LeaveWorld();
        }
    }

    public IEnumerator FadeInMushroomSpotlight()
    {//This backlight fades in as the mushroom burns away
        yield return new WaitForSeconds(19f);
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * .05f;
            mushroomSpotlight.intensity = Mathf.Lerp(0f, 2.37f, time);
            yield return null;
        }
        lightFinished = true;
    }

    public IEnumerator FadeOutLights()
    {//Fades out torch and scene lights not related to the mushroom, when the mushroom is burning
        flame.Stop();
        fireAS.Stop();

        foreach (ParticleSystem torch in wallTorches) { torch.Stop(); }
        foreach (Light flame in FlameLights) { flame.enabled = false; }

        if (SceneLightsArray != null)
        {
            foreach (Light sceneLight in SceneLightsArray)
            {
                GameObject gObject = sceneLight.gameObject;
                if (gObject.TryGetComponent(out LightFlickerEffect flickerScript)) { flickerScript.enabled = false; }

                float startingIntensity = sceneLight.intensity;
                sceneLight.intensity = 0f;
                yield return null;
            }
        }
    }

    void LeaveWorld() { if (!leftWorld) { leftWorld = true; StartCoroutine("LeaveWorldRoutine"); pressAnyButton.action.Disable(); pressAnyButton.action.performed -= context => LeaveWorld(); }  }

    IEnumerator LeaveWorldRoutine()
    {
        EndingText.SetActive(false);
        mushroomSpotlight.enabled = false;
        yield return new WaitForSeconds(5f);
        if (Application.CanStreamedLevelBeLoaded("TwoDoorScene"))
        {
            SceneManager.LoadScene("TwoDoorScene");
        }
    }

    public void PlayAudioClip(AudioSource audioS, AudioClip soundClip)
    {
        if (audioS != null && soundClip != null)
        {
            audioS.clip = soundClip;
            audioS.Play();
        }
    }

    public override void SetZone(Zone newlocation)
    {
        switch (newlocation)
        {
            case Zone.Cave:
                if (MainCaveSnapshot != null) { MainCaveSnapshot.TransitionTo(2.75f); }
                break;
            case Zone.Tunnel:
                if (MainForestSnapshot != null) { MainForestSnapshot.TransitionTo(2.75f); }
                break;
            case Zone.Forest:
                if (MainForestSnapshot != null) { MainForestSnapshot.TransitionTo(2.75f); }
                break;
            default:
                break;
        }
    }
}
