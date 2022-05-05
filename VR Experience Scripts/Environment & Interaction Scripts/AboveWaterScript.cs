using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboveWaterScript : MonoBehaviour
{

    public void OnTriggerEnter(Collider otherabove)
    {
        if (otherabove.tag == "Player")
        {
            SceneManagerScriptold.SceneManagerInstance.underwater = false;
            Debug.Log("currently above");
            SceneManagerScriptold.SceneManagerInstance.Invoke("Unmuted", 0f);
        }
    }
}
