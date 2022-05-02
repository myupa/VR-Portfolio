using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeapTools;

[System.Serializable]
public class EnvironmentObject
{
    public GameObject objectPrefab;
    [Range(0.0f, 1f)]
    public float sizeVariation = 0.1f;
    [Range(0.0f, 1f)]
    public float xZRotVariation = 0f;
    [Range(0.0f, 1f)]
    public float yRotVariation = 0f;
    public int quantity;
}

public class EnvironmentInstantiator : MonoBehaviour
{
    public float environRadius; //Width of area for spawning
    public float environHeight; //Max height to look for ground to spawn upon
    public int spawnLayer; //Which layer should game objects spawn upon

    public EnvironmentObject[] environmentObjects;

    void Start()
    {
        foreach (EnvironmentObject eObj in environmentObjects)
        {
            StartCoroutine(InstantiateAnObject(eObj));
        }
    }
    
    IEnumerator InstantiateAnObject(EnvironmentObject eObject)
    {
        int quantityReady = 0; //Quantity of game object created
        int layerMask = 1 << spawnLayer; //Layer to spawn onto

        while (quantityReady < eObject.quantity) //Stop when specific quantity has been created
        {
            bool instantiated = false;
            while (!instantiated)
            {
                Vector3 raycastCheckOrigin = new Vector3(Random.Range(-environRadius, environRadius), (environHeight - 1.25f), Random.Range(-environRadius, environRadius));
                //Make sure raycast origin isn't in the middle of another game object
                if (Physics.CheckSphere(raycastCheckOrigin, .01f) == false)
                {
                    //Raycasts downwards to find any game objects on a ground/etc layer.
                    Ray ray = new Ray(raycastCheckOrigin, -transform.up);
                    RaycastHit hitData;
                    if (Physics.Raycast(ray, out hitData, environHeight, layerMask))
                    {
                        instantiated = true;
                        //Suitable ground/etc layer was found
                        //Spawn the game object at the same x and z as the raycast origin
                        //With the y value from the raycast collision with the ground layer game object
                        float spawnY = hitData.point.y;
                        Vector3 spawnPoint = new Vector3(raycastCheckOrigin.x, spawnY, raycastCheckOrigin.z);
                        GameObject theObj = Instantiate(eObject.objectPrefab, spawnPoint, Quaternion.Euler(VariateSpawnRotation(eObject)), this.transform);
                        //Use the variation values to instantiate at varying rotations and scales
                        theObj.transform.localScale = VariateSize(eObject);
                        quantityReady++;
                        yield return null;
                    }
                    else //If raycast can't find the ground, this x/z isn't good for spawning a game object
                    {
                        yield return null;
                    }

                }
                else //If raycast origin is in another game object, this x/z might not be good for spawning a game object
                {
                    yield return null;
                }
            }

        }
    }

    Vector3 VariateSize(EnvironmentObject eObject)
    {
        float sizeVariationMultiplier = eObject.sizeVariation;
        if (sizeVariationMultiplier != 0f)
        {
            if (sizeVariationMultiplier == 1) { sizeVariationMultiplier = .9f; } // Value of 1 creates scale of 0
            float multiplier = Random.Range((1f - sizeVariationMultiplier), (1f + sizeVariationMultiplier));
            Vector3 origScale = eObject.objectPrefab.transform.lossyScale;
            return new Vector3(origScale.x * multiplier, origScale.y * multiplier, origScale.z * multiplier);
        }
        else //No variation specified
        {
            return eObject.objectPrefab.transform.lossyScale;
        }
    }

    Vector3 VariateSpawnRotation(EnvironmentObject eObject)
    {
        float rotx = eObject.xZRotVariation * 360f;
        float roty = eObject.yRotVariation * 360f;
        float rotz = eObject.xZRotVariation * 360f;
        Vector3 newRotation = new Vector3(Random.Range(0f, rotx), Random.Range(0f, roty), Random.Range(0f, rotz));
        return newRotation;
    }

}

