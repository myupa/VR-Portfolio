using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSceneManager : MonoBehaviour
{

    public GameObject textline;
    public GameObject playerController;
    Quaternion quaternion;
    void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
    }

    IEnumerator Start()
    {
        if (playerController != null) { 
        Vector3 eulerAngles = new Vector3(0f, 180f, 0f);
        quaternion.eulerAngles = eulerAngles;
        playerController.transform.rotation = quaternion; }

        yield return new WaitForSeconds(13f);
        if (textline != null) { textline.SetActive(false); }
    }
}
