using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonObjectStateSync : MonoBehaviourPunCallbacks
{
    //Sync individual objects without using a PhotonMasterSync inspector reference for them. 
    //Photon will only be aware of it if it has been enabled at least once.
    //Also, use to reset object position/rotation/velocity during gameplay.

    Vector3 objPosition;
    Quaternion objRotation;
    Rigidbody rb;

    public override void OnEnable() 
    { 
        if (PhotonMasterSync.instance != null) { PhotonMasterSync.instance.UpdateValue(gameObject.name, 1); } 
    
    }
    public override void OnDisable() 
    { 
        if (PhotonMasterSync.instance != null) { PhotonMasterSync.instance.UpdateValue(gameObject.name, 0); } 
    }


    void Start()
    {
        objPosition = gameObject.transform.position;
        objRotation = gameObject.transform.rotation;
        if (gameObject.GetComponent<Rigidbody>() != null) { rb = gameObject.GetComponent<Rigidbody>(); }
        SimulationEvents.instance.onReloadStates += ResetThroughEvent;
        SimulationEvents.instance.onResetItems += ResetThroughEvent;
    }

    void OnDestroy()
    {
        SimulationEvents.instance.onReloadStates -= ResetThroughEvent;
        SimulationEvents.instance.onResetItems -= ResetThroughEvent;
        if (!PhotonNetwork.OfflineMode && PhotonMasterSync.instance != null)
        {
            PhotonMasterSync.instance.UpdateValue(hashName, 0);
        }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
        {
            if (PhotonNetwork.InRoom) { photonView.RPC("Reset", RpcTarget.All); }
            else { Reset(); }
        }
    }

    void ResetThroughEvent()
    {
        if (PhotonNetwork.InRoom) { photonView.RPC("Reset", RpcTarget.All); }
        else { Reset(); }
    }

    [PunRPC]
    public void Reset()
    {
        if (rb != null) 
        { 
            rb.velocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
        }
        gameObject.transform.position = objPosition;
        gameObject.transform.rotation = objRotation;
    }
}
