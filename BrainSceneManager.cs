using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainSceneManager : SceneManagerParentClass
{
    private static BrainSceneManager _brainSM;
    public static BrainSceneManager brainSM
    {
        get
        {
            if (_brainSM == null) UnityEngine.Debug.LogError("Brain Scene Manager is null");
            return _brainSM;
        }
    }
    private void Awake() { _brainSM = this; }

    public GameObject line1;
    public Animator BrainAnimator;
    public AudioClip brainSpeech;
    public AudioSource brainspeechAS;
    [Range(0, 1)] public float brainspeechVol;

    public AudioClip heartBeat;
    public AudioSource heartBeatAS;
    [Range(0, 1)] public float heartBeatVolume;


    void Start()
    {
        StartCoroutine("Sequence");
    }

    IEnumerator Sequence()
    {
            float repetitions = 0f;
            while (repetitions < 10f)
            {
                repetitions ++;
                if (line1 != null) { line1.SetActive(true); }
                if (heartBeatAS != null && heartBeat != null) { heartBeatAS.PlayOneShot(heartBeat, heartBeatVolume); }
                if (BrainAnimator != null) { BrainAnimator.Play("Brainbeat"); }
                yield return new WaitForSeconds(1f);
                if (line1 != null) { line1.SetActive(false); }
                yield return new WaitForSeconds(1.2f);

                if (repetitions == 2) { brainspeechAS.PlayOneShot(brainSpeech, brainspeechVol); }

                if (repetitions == 6f)
                {
                    SceneTransitionManager.STManager.GoToScene("OfficeFish", 4f);
                }
            } 
    }
}
