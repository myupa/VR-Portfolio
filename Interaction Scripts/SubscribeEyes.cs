using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeEyes : MonoBehaviour
{
    Transform left;
    Transform right;

    void Start()
    {
        MushroomSceneManager.RendererFade += RunFadeOutShadows;
    }


    void OnDestroy()
    {
        MushroomSceneManager.RendererFade -= RunFadeOutShadows;
    }

    public void RunFadeOutShadows()
    {
        left = gameObject.transform.Find("L-EyeHinge");
        if (left != null) { left.gameObject.SetActive(false); }
        right = gameObject.transform.Find("R-EyeHinge");
        if (right != null) { right.gameObject.SetActive(false); }
    }

}
