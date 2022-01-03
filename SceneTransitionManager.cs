using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager _STManager;
    public static SceneTransitionManager STManager
    {
        get
        {
            if (_STManager == null) UnityEngine.Debug.LogError("Scene Transition Manager is null");
            return _STManager;
        }
    }

    public void Awake() { _STManager = this; }
    public void Start() { FadeSceneIn(FadeInLength); GetOriginalBlack(); }

    [Header("Starting Screen")]
    public GameObject ScreenImageGO;
    public float FadeInLength = 5.5f;
    
    [Header("Start Prompt Screen?")]
    public bool usesStartPrompt;
    Color originalColor;
    public Color promptColor;
    private Color altColor;

    [Header("Audio")]
    public AudioMixerSnapshot MasterMuted;
    public AudioMixerSnapshot MasterUnmuted;

    public void GoToScene(string sceneName, float transitionLength)
    {
        if (transitionLength >= .1) 
        { 
        StartCoroutine(FadeSceneOutRoutine(transitionLength));
        StartCoroutine(FadeSceneAudioOutRoutine(transitionLength));
        StartCoroutine(SceneSwitch(sceneName, transitionLength));
        }
        if (transitionLength == 0)
        {
            StartCoroutine(SceneSwitch(sceneName, transitionLength));
        }
    }

    public void FadeSceneIn(float length) { if (!usesStartPrompt) { GetOriginalBlack(); StartCoroutine(FadeSceneInRoutine(length)); } else { StartCoroutine(FadeStartPromptIn(length)); } }
    public void FadeSceneIn() { StartCoroutine(FadeSceneInRoutine(1f)); }
    public void FadeSceneOut(float length) { StartCoroutine(FadeSceneOutRoutine(length)); }
    public void FadeSceneOut() { StartCoroutine(FadeSceneOutRoutine(1f)); }
    public void FadeOutPrompt(float length) { if (usesStartPrompt) { StartCoroutine(FadeOutPromptRoutine(length)); } }
    void GetOriginalBlack() { if (ScreenImageGO != null) { Image image = ScreenImageGO.GetComponent<Image>(); originalColor = image.color; } }

    public IEnumerator FadeSceneInRoutine(float length)
    {
        yield return new WaitForSeconds(1f);
        if (MasterUnmuted != null) { MasterUnmuted.TransitionTo(length); }
        if (ScreenImageGO != null)
        {
            
            Image image = ScreenImageGO.GetComponent<Image>();
            var tempColor = image.GetComponent<Image>().color;
            float multiplier = 1 / length;
            float time = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime * multiplier;
                tempColor.a = Mathf.Lerp(1f, 0f, time);
                image.color = tempColor;
                yield return null;
            }
        }
    }

    public IEnumerator FadeSceneOutRoutine(float length)
    {
        if (ScreenImageGO != null)
        {
            Image image = ScreenImageGO.GetComponent<Image>();
            originalColor.a = 0f; image.color = originalColor;
            var tempColor = image.GetComponent<Image>().color;
            float multiplier = 1 / length;

            float time = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime * multiplier;
                tempColor.a = Mathf.Lerp(0f, 1f, time);
                image.color = tempColor;
                yield return null;
            }
        }
    }

    public IEnumerator FadeSceneAudioOutRoutine(float length)
    {
        if (MasterMuted != null)
        {
            float actualLength = length * .75f;
            //yield return new WaitForSeconds(actualLength);
            //MasterMuted.TransitionTo(actualLength); 
            MasterMuted.TransitionTo(actualLength);
            yield return null;
        }
    }

    public IEnumerator FadeStartPromptIn(float length)
    {
        if (MasterUnmuted != null) { MasterUnmuted.TransitionTo(length); }
        if (ScreenImageGO != null)
        {
            Image image = ScreenImageGO.GetComponent<Image>();
            originalColor = image.color;
            float multiplier = 1 / length;
            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * multiplier;
                image.color = Color.Lerp(originalColor, promptColor, time);
                yield return null;
            }
            //var tempColor = image.GetComponent<Image>().color;
            //tempColor = originalColor;
        }
    }

    public IEnumerator FadeOutPromptRoutine(float length)
    {
        if (ScreenImageGO != null)
        {
            Image image = ScreenImageGO.GetComponent<Image>();
            var tempColor = image.GetComponent<Image>().color;
            float multiplier = 1 / length;
            float time = 0f;
            float startingA = tempColor.a;
            while (time < 1f)
            {
                time += Time.deltaTime * multiplier;
                tempColor.a = Mathf.Lerp(startingA, 0f, time);
                image.color = tempColor;
                yield return null;
            }
        }
    }

    private IEnumerator SceneSwitch(string sceneName, float transitionLength)
    {
        yield return new WaitForSeconds(transitionLength);
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        if (Application.CanStreamedLevelBeLoaded(sceneName)) { SceneManager.LoadScene(sceneName, LoadSceneMode.Single); }
    }

}
