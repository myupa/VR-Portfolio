using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PunSingleStateSync : MonoBehaviourPunCallbacks
{
    //Sync individual objects without using an inspector reference for them. 
    //Photon will only be aware of it if it has been enabled at least once.

    string hashName;
    GameObject theObject;

    void Awake()
    { 
        if (!PhotonNetwork.OfflineMode) { hashName = gameObject.name; } 
    }

    public override void OnEnable() 
    { 
        if (PunObjectSync._ObjectSync != null) { PunObjectSync._ObjectSync.UpdateValue(hashName, 1); }
    }

    public override void OnDisable() 
    {
        if (PunObjectSync._ObjectSync != null) { PunObjectSync._ObjectSync.UpdateValue(hashName, 0); }
    }

    void OnDestroy() 
    { 
        if (!PhotonNetwork.OfflineMode && PunObjectSync._ObjectSync != null) { PunObjectSync._ObjectSync.UpdateValue(hashName, 0); }  
    }
    
}
