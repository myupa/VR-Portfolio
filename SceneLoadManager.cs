using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
public enum SceneToLoad
{
    Scene1,
    Scene2,
    Scene3,
    DoorScene
}*/

public class SceneLoadManager : MonoBehaviour
{/*
    private static SceneLoadManager _sceneloadmanagerinstance;
    public static SceneLoadManager SceneLoadManagerInstance
    {
        get
        {
            if (_sceneloadmanagerinstance == null)
                UnityEngine.Debug.LogError("Scene Manager is null");

            return _sceneloadmanagerinstance;
        }
    }

    private void Awake()
    {
        if (_sceneloadmanagerinstance != null)
        {
            Destroy(gameObject);
            return;
        }

        _sceneloadmanagerinstance = this;
        DontDestroyOnLoad(gameObject);
    }

    string sceneString;
    Image FadeScreen;

    public void LoadScene(SceneToLoad sceneToLoad)
    {
        
        switch (sceneToLoad)
        {
            case SceneToLoad.Scene1:
                sceneString = "DoorScene";
                break;
            case SceneToLoad.Scene2:
                sceneString = "Extant";
                break;
            case SceneToLoad.Scene3:
                sceneString = "DoorScene";
                break;
            case SceneToLoad.DoorScene:
                sceneString = "DoorScene";
                break;
            default:
                break;
        }
        StartCoroutine(LoadSceneRoutine(sceneString));
    }

    public IEnumerator LoadSceneRoutine(string sceneString)
    {
        if (FadeScreen == null)
        {
            GameObject FadeScreenGO = GameObject.Find("FadeScreen");
            FadeScreen = FadeScreenGO.GetComponent<Image>();
            float time = 0f;
            var tempColor = FadeScreenGO.GetComponent<Image>().color;
            while (time < 1f)
            {
                time += Time.deltaTime *2f;
                tempColor.a = Mathf.Lerp(.4f, 1f, time);
                FadeScreen.color = tempColor;
                yield return null;
            }
        }
        if (Application.CanStreamedLevelBeLoaded(sceneString)) { SceneManager.LoadScene(sceneString); }
    }

    

    //BNG.ScreenFader sf;

    public void LoadScene(SceneToLoad sceneToLoad) 
    {
        switch (sceneToLoad)
        {
            case SceneToLoad.Scene1:
                sceneString = "scene1";
                AsyncOperation asyncUnload1 = SceneManager.UnloadSceneAsync("scene2");
                AsyncOperation asyncUnload2 = SceneManager.UnloadSceneAsync("scene3");
                StartCoroutine(StartLoading(sceneString));
                break;
            case SceneToLoad.Scene2:
                sceneString = "Extant";
                //AsyncOperation asyncUnload3 = SceneManager.UnloadSceneAsync("scene1");
                //AsyncOperation asyncUnload4 = SceneManager.UnloadSceneAsync("scene3");
                StartCoroutine(StartLoading(sceneString));
                break;
            case SceneToLoad.Scene3:
                sceneString = "scene3";
                AsyncOperation asyncUnload5 = SceneManager.UnloadSceneAsync("scene1");
                AsyncOperation asyncUnload6 = SceneManager.UnloadSceneAsync("scene2");
                StartCoroutine(StartLoading(sceneString));
                break;
            default:
                break;
        }

    }

    public IEnumerator StartLoading(string sceneString)
    {
        yield return null; 

        if (Application.CanStreamedLevelBeLoaded(sceneString)) {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            //if (sf == null) { sf = FindObjectOfType<ScreenFader>();
             //   if (sf != null) { sf.DoFadeIn(); } }

            //yield return new WaitForSeconds(.5f);

    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneString);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f) { yield return null; }
        while (!switchTriggered) { yield return null; }

        asyncOperation.allowSceneActivation = true;


   // SceneLoadManager.SceneLoadManagerInstance.LoadScene(SceneToLoad);

    //while (!asyncOperation.isDone)
    //     {
   //          if (asyncOperation.progress >= .9f && switchTriggered)
    //         {
    //             asyncOperation.allowSceneActivation = true;
    //         }
    //     }

    }
}*/
}
