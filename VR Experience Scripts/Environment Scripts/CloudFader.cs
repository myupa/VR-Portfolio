using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudFader : MonoBehaviour
{
    Renderer cloudtransp;

    public void Start()
    {
        GoddessSceneManager.FadeInClouds += FadeCloudsIn; 
        cloudtransp = gameObject.GetComponent<Renderer>();
    }

    public void OnDestroy()
    {
        GoddessSceneManager.FadeInClouds -= FadeCloudsIn;
    }

    public void FadeCloudsIn() { if (cloudtransp != null) { StartCoroutine("FadeCloudsRoutine"); } }

    public IEnumerator FadeCloudsRoutine()
    {
        float time = 0f;
        var tempColor = cloudtransp.GetComponent<Renderer>().material.color; 
        while (time < 1f)
        {
            time += Time.deltaTime * .1f;
            tempColor.a = Mathf.Lerp(0f, .74f, time);
            cloudtransp.material.color = tempColor;
            yield return null;
        }
    }
}


