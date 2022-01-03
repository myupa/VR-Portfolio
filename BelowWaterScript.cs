using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelowWaterScript : MonoBehaviour
{
    public void OnTriggerEnter(Collider otherbelow)
    {
        if (otherbelow.tag == "Player")
        {
            SceneManagerScriptold.SceneManagerInstance.underwater = true;
            Debug.Log("currently below");
            SceneManagerScriptold.SceneManagerInstance.Invoke("QuickMuffleAudio", 0f);
        }
    }
}
