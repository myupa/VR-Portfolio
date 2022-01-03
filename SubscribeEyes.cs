using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeEyes : MonoBehaviour
{
    void Start()
    {
        MushroomSceneManager.RendererFade += RunFadeOutShadows;
    }

    public void RunFadeOutShadows()
    {
        gameObject.SetActive(false);
    }
}
