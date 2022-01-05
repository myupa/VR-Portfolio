using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PunObjectSync : MonoBehaviourPunCallbacks
{
    //This scripts syncs values, object states, and animations, for multiplayer experiences built with Photon and Unity.
    //Use "add value/data/etc" methods to create and assign values, usually when the master client is creating a room and hasn't yet uploaded a hashtable.
    //Use "update value/data/etc" methods for values added or updated after the master client created a room. This will catch up values for players that join later.

    public GameObject[] sensors; //In-game sensors. Photon needs to always be aware of them through inspector reference.
    public GameObject[] uniqueObjects; //Objects important to gameplay. Photon needs to always be aware of them through inspector reference.
    public Animator animator; 
    private ExitGames.Client.Photon.Hashtable roomHashTable = new ExitGames.Client.Photon.Hashtable();

    public static PunObjectSync _ObjectSync;

    [Button]
    void PrintAll()
    {
        roomHashTable = PhotonNetwork.CurrentRoom.CustomProperties;
        print(roomHashTable);
    }

    private void Awake() 
    {
        if (_ObjectSync == null) { _ObjectSync = this; }
        else if (_ObjectSync != this) { Destroy(gameObject); }
    }

    void Start() { SetSensorValues(); }

    public override void OnJoinedRoom()
    {
        InitiateHashTable(); // Only Master Client will automatically set starting values of hashtable.
        SyncHashTable(); // Non-master clients will sync their local hashtable to the photon room's values.
        SyncSensorValues(); 
        SyncAnimState(animator.ToString());
        SyncSingleStates();
    }

    public override void OnLeftRoom() //The below code deletes the Photon hashtable when master client leaves. 
    {
        if (PhotonNetwork.IsMasterClient) { PhotonNetwork.CurrentRoom.SetCustomProperties(null); }
        roomHashTable = null;
    }

    public void InitiateHashTable() 
    {
        if (!PhotonNetwork.OfflineMode) 
        { 
            if (PhotonNetwork.IsMasterClient) { PhotonNetwork.CurrentRoom.SetCustomProperties(roomHashTable); } 
        }
    }

    public void SyncHashTable() // Syncing values pulls directly from uploaded hashtable.
    {
        if (!PhotonNetwork.OfflineMode) 
        { 
            if (!PhotonNetwork.IsMasterClient) { roomHashTable = PhotonNetwork.CurrentRoom.CustomProperties; }
        }
    }

    public void AddValue(string hashName, int value)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (roomHashTable[hashName] == null) { roomHashTable.Add(hashName, value); }
            else { roomHashTable[hashName] = value; }
        }
    }

    public void UpdateValue(string hashName, int value)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (PhotonNetwork.InRoom)
            {
                if (roomHashTable[hashName] != null) { roomHashTable[hashName] = value; }
                else { roomHashTable.Add(hashName, value); }
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomHashTable);
            }
        }
    }

    public void AddData(string hashName, float data)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (roomHashTable[hashName] == null) { roomHashTable.Add(hashName, data); }
            else { roomHashTable[hashName] = data; }
        }
    }

    public void UpdateData(string hashName, float data)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (PhotonNetwork.InRoom)
            {
                if (roomHashTable[hashName] != null) { roomHashTable[hashName] = data; }
                else { roomHashTable.Add(hashName, data); }
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomHashTable);
            }
        }
    }

    public void AddAnimState(string animatorName, int animState) //Script on anim states call this method
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (roomHashTable[animatorName] == null) { roomHashTable.Add(animatorName, animState);  }
            else { roomHashTable[animatorName] = animState; }
        }
    }

    public void UpdateAnimState(string animatorName, int animState) //Script on anim states call this method
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (roomHashTable[animatorName] != null) { roomHashTable[animatorName] = animState; }
            else { roomHashTable.Add(animatorName, animState); }
            if (PhotonNetwork.InRoom) { PhotonNetwork.CurrentRoom.SetCustomProperties(roomHashTable); }
        }
    }

    public void SyncAnimState(string animatorName)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties[animatorName] == null) { print(animatorName + "is null"); }
            else { int State = (int)PhotonNetwork.CurrentRoom.CustomProperties[animatorName]; animator.Play(State, 0, 1f); }
        }
    }

    void SetSensorValues()
    {
        foreach (GameObject sensor in sensors)
        {
            if (sensor.activeSelf == false)
            {
                int State = 0; roomHashTable.Add(sensor.name, State); 
            }
            if (sensor.activeSelf == true)
            {
                int State = 1; roomHashTable.Add(sensor.name, State);
            }
        }
    }

    void SyncSensorValues()
    {
        if (!PhotonNetwork.OfflineMode) { if (!PhotonNetwork.IsMasterClient)
            {
                foreach (GameObject sensor in sensors)
                {
                    string hashstring = sensor.name; object value;
                    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(hashstring, out value))
                    {
                        if (value is 0) { sensor.SetActive(false); }
                        if (value is 1) { sensor.SetActive(true); }
                    }
                    else { sensor.SetActive(false); }
                }
            } }
    } 

    void SyncSingleStates() //Syncing the state of unique objects -- active or inactive.
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                foreach (GameObject thing in uniqueObjects)
                {
                    string hashstring = thing.name; object state;
                    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(hashstring, out state))
                    {
                        if (state is 0) { thing.SetActive(false); }
                        if (state is 1) { thing.SetActive(true); }
                    }
                }
            }
        }
    }

    void CheckValues()
    {
        print("localhash: " + roomHashTable);
        print("networkedhash: " + PhotonNetwork.CurrentRoom.CustomProperties);
    }

}
