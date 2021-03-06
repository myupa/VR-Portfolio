using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRToolsManager : MonoBehaviour
{

    public static VRToolsManager instance;
    private void Awake() { instance = this; }
    public BNG.SceneLoader sceneLoader;

    //Guide Hand Tools

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
        if (!showingGuideHands) { ShowGuideHands(); showingGuideHands = true; }
        else { HideGuideHands(); showingGuideHands = false; }
    }


    //Toggle isTrigger Guide Regions
    //

    Collider[] Regions;
    List<GameObject> Highlights = new List<GameObject>();
    bool showingRegions = false;

    public void ToggleRegions()
    {
        if (!showingRegions) { ShowRegions(); showingRegions = true; }
        else { HideRegions(); showingRegions = false; }
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

    //Toggle Underwater Scenery & Tools
    //
    public void GoUnderwater()
    {
        Physics.gravity = new Vector3(0f, -.75f, 0f); 
        if (sceneLoader != null) { sceneLoader.LoadScene("VR Underwater"); }
    }

    public void LeaveUnderwater()
    {
        Physics.gravity = new Vector3(0f, -7.81f, 0f);
        if (sceneLoader != null) { sceneLoader.LoadScene("VR Tools"); }
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
