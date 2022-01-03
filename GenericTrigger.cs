using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenericTrigger : MonoBehaviour
{
    [Header("Function")]
    public SceneManagerParentClass LocalSceneManager;
    public bool reusableTrigger;
    public string TriggerTag;
    Collider triggercollider;
    public string DebugMessage;

    [Header("Actions")]
    public string NameOfFunction;
    public GameObject toActivate;
    public GameObject toDeactivate;

    [Header("Location")]
    public bool SetZone;
    public Zone setLocation;

    public void Start()
    {
        if (!triggercollider) { triggercollider = gameObject.GetComponent<Collider>(); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TriggerTag)
        {
            if (!reusableTrigger) { triggercollider.enabled = false; }
            if (CheckString(NameOfFunction)) { if (LocalSceneManager != null) { LocalSceneManager.Invoke(NameOfFunction, 0f); } }
            if (CheckString(DebugMessage)) { Debug.Log(DebugMessage); }
            Invoke("Actions", 1f);
            if (SetZone && LocalSceneManager != null) { LocalSceneManager.SetZone(setLocation); }
        }
    }

    void Actions()
    {
        if (toActivate != null) { toActivate.SetActive(true); }
        if (toDeactivate != null) { toDeactivate.SetActive(false); }
    }

    bool CheckString(string s)
    {
        if (String.IsNullOrEmpty(s)) { return false; }
        else { return true; }
    }

}
