using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneTrigger : MonoBehaviour
{
    public string sceneToLoad;
    bool triggered = false;
    public float transitionLength;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !triggered)
        {
            triggered = true;
            SceneTransitionManager.STManager.GoToScene(sceneToLoad, transitionLength);
        }
    }
}
