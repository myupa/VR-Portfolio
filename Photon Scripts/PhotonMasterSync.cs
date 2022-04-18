using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Sirenix.OdinInspector;

public class PunObjectSync : MonoBehaviourPunCallbacks
{
    //This scripts syncs values, object states, and animations, for multiplayer experiences built with Photon and Unity.
    //Use "add value/data/etc" methods to create and assign values, usually when the master client is creating a room and hasn't yet uploaded a hashtable.
    //Use "update value/data/etc" methods for values added or updated after the master client created a room. This will catch up values for players that join later.

    [Header("Game Objects")]
    public GameObject[] sensors; //In-game sensors
    public GameObject[] uniqueObjects; //Non-instantiated objects important to gameplay
    private ExitGames.Client.Photon.Hashtable initHashTable = new ExitGames.Client.Photon.Hashtable(); //To locally save object/sensor states from scene start
    private ExitGames.Client.Photon.Hashtable roomHashTable = new ExitGames.Client.Photon.Hashtable(); //To sync with Photon

    [Header("Animations")]
    public Animator animator;
    bool initialAnimStateSaved = false;

    public static PhotonMasterSync instance;

    private void Awake() 
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
    }

    void Start() 
    { 
        SetInitialSensorStates(); //Saves initial states of Sensors to local hashtable for each player. Reloading states uses these values.
        SetInitialUniqueObjectStates(); //Saves initial states of Single Objects to local hashtable for each player. Reloading states uses these values.
        if (SimulationEvents.instance != null)
        {
            SimulationEvents.instance.onReloadStates += ResetSensors;
            SimulationEvents.instance.onReloadStates += ResetSingleStates;
            SimulationEvents.instance.onReloadStates += ResetAnimState;
        }
    }

    private void OnDisable()
    {
        base.OnDisable();
        if (SimulationEvents.instance != null)
        {
            SimulationEvents.instance.onReloadStates -= ResetSensors;
            SimulationEvents.instance.onReloadStates -= ResetSingleStates;
            SimulationEvents.instance.onReloadStates -= ResetAnimState;
        }
    }

    [Button][PunRPC] //Called as RPC, to be used by each player by syncing to their (identical) local initHashTables
    public void Reset() { SimulationEvents.instance.OnReloadStates(); }

    public override void OnJoinedRoom()
    {
        InitiateHashTable(); // Only Master Client will automatically set starting values of hashtable.
        SyncHashTable(); // Non-master clients will sync their local hashtable to the photon room's values.
        SyncSensorValues(); 
        SyncAnimState(animator.ToString());
        SyncSingleStates();
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

    public void SetInitialValue(string HashName, int Value)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (initHashTable[HashName] == null) { initHashTable.Add(HashName, Value); }
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
            if (PhotonNetwork.CurrentRoom.CustomProperties[animatorName] == null) { print(animatorName + " anim Hashtable value missing."); }
            else 
            { 
                int state = (int)PhotonNetwork.CurrentRoom.CustomProperties[animatorName]; 
                animator.Play(state, 0, 1f); 
            }
        }
    }

    public void ResetAnimState()
    {
        int state;
        if (initHashTable[animator.ToString()] != null)
        {
            state = (int)initHashTable[animator.ToString()];
            animator.Play(state, 0, 0f);
        }
    }

    void SetInitialSensorStates()
    {
        foreach (GameObject sensor in sensors)
        {
            int state;
            if (sensor.activeSelf == true ) { state = 1; }
            else { state = 0; }
            roomHashTable.Add(sensor.name, state); 
            initHashTable.Add(sensor.name, state);
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

    public void ResetSensors() 
    {
        if (!PhotonNetwork.OfflineMode)
        {
            foreach (GameObject sensor in sensors) { sensor.SetActive(false); }
        }
    }

    void SetInitialUniqueObjectStates()
    {
        foreach (GameObject thing in uniqueObjects)
        {
            int state;
            if (thing.activeSelf == true) { state = 0; }
            else { state = 1; }
            initHashTable.Add(thing.name, state); 
            roomHashTable.Add(thing.name, state);
        }
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

    public void ResetSingleStates()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            foreach (GameObject thing in SingleObjects)
            {
                string hashstring = thing.name; object state;
                if (initHashTable.TryGetValue(hashstring, out state))
                {
                    if (state is 0) { thing.SetActive(false); }
                    if (state is 1) { thing.SetActive(true); }
                }
                else { }
            }
        }
    }

    [Button]
    void CheckValues()
    {
        print("Local hashtable: " + roomHashTable);
        print("Networked hashtable: " + PhotonNetwork.CurrentRoom.CustomProperties);
        print("Initial hashtable " + initHashTable);
    }

}
