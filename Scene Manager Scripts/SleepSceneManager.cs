using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepSceneManager : MonoBehaviour
{
    
    public void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(20f);
        SceneTransitionManager.STManager.GoToScene("TwoDoorScene", 10f);
    }
}
