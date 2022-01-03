using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShadowController : MonoBehaviour
{
    //ExternalKill is called by the Sequence 2 routine in MainSceneManager
    //This script also turns off the IK that makes shadows stare at player, when they enter the tunnel

    public GameObject character;
    public GameObject[] rendererGO;
    public Animator animator;
    public GameObject NavGO;
    public GameObject PMaster;
    public RootMotion.FinalIK.LookAtIK IKScript;
    [HideInInspector] public bool pastThreshold = false;
    RootMotion.Dynamics.PuppetMaster puppetMaster;
    RootMotion.Demos.NavMeshPuppet navPuppet;
    NavMeshAgent navAgent;
    public AudioSource shadowAS;
    bool walking = false;
    string IKTriggerName;

    public void Start()
    {
        if (character != null) { puppetMaster = character.GetComponent<RootMotion.Dynamics.PuppetMaster>(); }
        if (NavGO != null) { navPuppet = NavGO.GetComponent<RootMotion.Demos.NavMeshPuppet>(); }
        if (NavGO != null) { navAgent = NavGO.GetComponent<NavMeshAgent>(); }
        if (navAgent != null) { navAgent.isStopped = true; }
        IKTriggerName = navPuppet.target.name;
        MushroomSceneManager.StartNavAgents += StartWalking;
        MushroomSceneManager.StopNavAgents += StopWalking;
        MushroomSceneManager.SpeedUpNavAgents += SpeedUpWalking;
        MushroomSceneManager.KillOffShadows += KillOffShadow;
        MushroomSceneManager.ShadowsSpeak += Speak;
        //MushroomSceneManager.ShadowsGoQuiet += ShutUp;
    }

    public void OnDestroy()
    {
        MushroomSceneManager.StartNavAgents -= StartWalking;
        MushroomSceneManager.StopNavAgents -= StopWalking;
        MushroomSceneManager.SpeedUpNavAgents -= SpeedUpWalking;
        MushroomSceneManager.KillOffShadows -= KillOffShadow;
        MushroomSceneManager.ShadowsSpeak -= Speak;
        //MushroomSceneManager.ShadowsGoQuiet -= ShutUp;
    }

    void Speak()
    {
        StartCoroutine("SpeakRoutine");
    }

    IEnumerator SpeakRoutine()
    {
        if (shadowAS != null)
        {
            int reps = 0;
            float random;
            float randomrangemin = Random.Range(2f, 4f);
            float randomrangemax = Random.Range(5f, 8f);
            while (reps < 7)
            {
                random = Random.Range(randomrangemin, randomrangemax);
                yield return new WaitForSeconds(random);
                reps++;
                shadowAS.Play();
                yield return null;
            }
        }
    }

    void ShutUp()
    {
        StopCoroutine("SpeakRoutine");
    }

    void StartWalking()
    {
        if (animator != null && navAgent != null)
        {
            walking = true;
            StartCoroutine("StartWalkingRoutine");
        }
    }

    IEnumerator StartWalkingRoutine()
    {
        float wait = UnityEngine.Random.Range(0f, 1.5f);
        yield return new WaitForSeconds(wait);
        //navAgent.speed = .65f;
        navAgent.isStopped = false;
        animator.SetTrigger("StartWalking");
    }

    void StopWalking()
    {
        if (animator != null && navAgent != null)
        {
            navAgent.isStopped = true;
            animator.SetTrigger("StopWalking");
            puppetMaster.SwitchToKinematicMode();
        }
    }

    void SpeedUpWalking()
    {
        if (animator != null && navAgent != null)
        {
            //navAgent.speed = .9f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IKTrigger")
        {
            StartCoroutine("StopLookingAtPlayer");
        }
        if (walking = true && other.name == IKTriggerName)
        {
            walking = false;
            StopWalking();
        }
    }

    public IEnumerator StopLookingAtPlayer()
    {
        pastThreshold = true;
        ShutUp();
        if (IKScript != null)
        {
            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * .5f;
                IKScript.solver.IKPositionWeight = Mathf.Lerp(1f, 0f, time);
                yield return null;
            }
            IKScript.solver.IKPositionWeight = 0f;
        }
    }

    public void ExternalKill()
    {
        StartCoroutine("ShadowDeathRoutine"); // Currently the one used of this and the below method
    }

    public void KillOffShadow()
    {
        StartCoroutine("ShadowDeathRoutine");
    }

    bool randomBool
    {
        get  { return (Random.value > 0.49f); }
    }

    public IEnumerator ShadowDeathRoutine()
    {
        float wait = UnityEngine.Random.Range(.5f, 2f);
        float deathSpeed = Random.Range(.7f, 1.3f);
        int deathtype = Random.Range(0, 5);
        yield return new WaitForSeconds(wait);
        if (puppetMaster != null)
        {
            animator.SetFloat("DeathSpeed", deathSpeed);
            if (deathtype == 0) { animator.SetTrigger("Death"); print("a"); } 
            if (deathtype == 1) { animator.SetTrigger("Death2"); print("b"); } 
            if (deathtype == 2) { animator.SetTrigger("Death3"); print("c"); } 
            if (deathtype == 3) { animator.SetTrigger("Death4"); print("d"); } 
            if (deathtype == 4) { animator.SetTrigger("Death5"); print("e"); } 
            if (deathtype == 5) { animator.SetTrigger("Death6"); print("f"); } 

            puppetMaster.SwitchToKinematicMode();
        }

        foreach (GameObject renderer in rendererGO)
        {
            if (renderer != null)
            {
                SkinnedMeshRenderer shadow = renderer.GetComponent<SkinnedMeshRenderer>();
                //shadow.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        yield return new WaitForSeconds(7f);

        if (navPuppet != null) { navPuppet.enabled = false; }
        if (navAgent != null) { navAgent.enabled = false; }
        if (puppetMaster != null) { puppetMaster.enabled = false; }

        if (PMaster != null) { PMaster.SetActive(false); }
    }
}
