using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRToolsManager : MonoBehaviour
{

    public static VRToolsManager instance;
    private void Awake() { instance = this; }
    bool allowButtons = false;

    //Guide Hand Tools
    //
    public enum HandDominance { RightHanded, LeftHanded, Ambidextrous}
    [Header("Grab Point Guides to Show")]
    public HandDominance dominantHand;
    [HideInInspector] public bool showingGuideHands = false;

    public void SetLefty() { dominantHand = HandDominance.LeftHanded; }
    public void SetRighty() { dominantHand = HandDominance.RightHanded; }

    public event Action showGuideHands;
    public void ShowGuideHands()
    {
        if (showGuideHands != null)
        {
            showGuideHands();
            showingGuideHands = true;
        }
    }

    public event Action hideGuideHands;
    public void HideGuideHands()
    {
        if (hideGuideHands != null)
        {
            hideGuideHands();
            showingGuideHands = false;
        }
    }

    public void ToggleHands()
    {
        if (allowButtons)
        {
            if (!showingGuideHands) { ShowGuideHands(); showingGuideHands = true; }
            else { HideGuideHands(); showingGuideHands = false; }
        }
    }


    //Toggle isTrigger Guide Regions
    //

    Collider[] Regions;
    List<GameObject> Highlights = new List<GameObject>();
    bool showingRegions = false;

    public void ToggleRegions()
    {
        if (allowButtons)
        {
            if (!showingRegions) { ShowRegions(); showingRegions = true; }
            else { HideRegions(); showingRegions = false; }
        }
    }

    void ShowRegions()
    {
        Regions = GameObject.FindObjectsOfType<Collider>();
        foreach (Collider col in Regions)
        {
            if (col.isTrigger == true)
            {
                BuildRegionHighlight(col.gameObject);
            }
        }
    }

    void HideRegions()
    {
        foreach (GameObject g in Highlights)
        {
            Destroy(g);
        }
    }

    void BuildRegionHighlight(GameObject g)
    {
        GameObject guidingRegion = Instantiate(Resources.Load("GuidingRegionPrefab") as GameObject);
        guidingRegion.layer = 31;
        guidingRegion.transform.position = g.transform.position;
        guidingRegion.transform.rotation = g.transform.rotation;
        guidingRegion.transform.localScale = new Vector3(.15f, .15f, .15f); 
        guidingRegion.transform.parent = g.transform;
        Highlights.Add(guidingRegion);
    }

    //Toggle Underwater Mode & Tools
    //
    [Header("Underwater Components")]
    public GameObject Whale; 
    public MakeWaves[] WaveComponents;
    bool isUnderwater = false;

    public void ToggleUnderwater()
    {
        if (allowButtons)
        {
            if (!isUnderwater) { GoUnderwater(); isUnderwater = true; }
            else { LeaveUnderwater(); isUnderwater = false; }
        }

    }

    public void GoUnderwater()
    {
        foreach(MakeWaves makeWaves in WaveComponents)
        {
            makeWaves.enabled = true;
        }
        if (Whale != null) { Whale.SetActive(true); }
        RenderSettings.fog = true;
        Physics.gravity = new Vector3(0f, -.25f, 0f);
    }

    public void LeaveUnderwater()
    {
        foreach (MakeWaves makeWaves in WaveComponents)
        {
            makeWaves.enabled = false;
        }
        if (Whale != null) { Whale.SetActive(false); }
        RenderSettings.fog = false;
        Physics.gravity = new Vector3(0f, -7.81f, 0f);
    }

    //Reset Item Position/Rotations
    //

    public event Action resetObjectState;
    public void ResetObjectState()
    {
        if (resetObjectState != null)
        {
            resetObjectState();
        }
    }
}
