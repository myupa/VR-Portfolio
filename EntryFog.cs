using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryFog : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SceneManagerScriptold.SceneManagerInstance.FogFade());
    }

}
