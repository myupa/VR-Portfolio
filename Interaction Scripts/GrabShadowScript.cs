using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabShadowScript : MonoBehaviour
{
    public Collider thisLimb;
    public RootMotion.Demos.NavMeshPuppet NavScript;
    float originalSpeed;
    public RootMotion.FinalIK.LookAtIK IKScript;
    public ShadowController ShadowController;

    void Start()
    {
        if(thisLimb == null) { gameObject.GetComponent<Collider>(); }
        if(NavScript != null) { originalSpeed = NavScript.agent.speed; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HandGrabTrigger" && ShadowController != null)
        {
            if (thisLimb != null)
            {
                if (NavScript != null) { NavScript.agent.speed = 0f; }
                if (IKScript != null && !ShadowController.pastThreshold) { StartCoroutine("LerpDownWeight"); }
            }
        }

        if (other.tag == "HandReleaseTrigger" && ShadowController != null)
        {
            if (thisLimb != null)
            {
                if (NavScript != null) { NavScript.agent.speed = originalSpeed;}
                if (IKScript != null && !ShadowController.pastThreshold) { StartCoroutine("LerpUpWeight"); }
            }
        }
    }

    IEnumerator LerpDownWeight()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            IKScript.solver.IKPositionWeight = Mathf.Lerp(1f, 0f, time);
            yield return null;
        }
        IKScript.solver.IKPositionWeight = 0f;
    }

    IEnumerator LerpUpWeight()
    {
        float time = 0f;
        while(time < 1f)
        {
            time += Time.deltaTime;
            IKScript.solver.IKPositionWeight = Mathf.Lerp(0f, 1f, time);
            yield return null;
        }
        IKScript.solver.IKPositionWeight = 1f;
    }
}
