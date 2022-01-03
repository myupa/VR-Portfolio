using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSceneManager : MonoBehaviour
{

    public GameObject textline;
    void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(13f);
        if (textline != null) { textline.SetActive(false); }
    }
}
