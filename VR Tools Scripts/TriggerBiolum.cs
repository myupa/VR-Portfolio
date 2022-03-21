using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBiolum : MonoBehaviour
{
    public BiolumController controller;

    //Trigger bioluminscence through touch or water waves from anywhere on multi-boned game object

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wave" || other.gameObject.tag == "PlayerHand")
        {
            if (controller != null) { controller.Glow(); }
        }

    }
}
