using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiolumController : MonoBehaviour
{

    //Give an object, with an emissive material, the ability to look bioluminscent upon physical contact

    public SkinnedMeshRenderer meshRenderer;
    Material mat;
    public float multiplier = 4f;
    bool allowGlow = true;
    Color emissiveLumin; //Brightest & greenest emissiveness
    Color originalLumin; //Original emissiveness

    public void Start()
    {
        if (meshRenderer != null)
        {
            mat = meshRenderer.material;
            emissiveLumin = mat.GetColor("_EmissionColor");
            originalLumin = mat.GetColor("_EmissionColor");
            emissiveLumin.g = emissiveLumin.g * multiplier;
        }
        else { print(gameObject.name + "'s Biolum Controller is missing a reference!"); }
    }

    public void Glow()
    {
        if (allowGlow && mat != null) { StartCoroutine("GlowRoutine"); }
    }

    IEnumerator GlowRoutine()
    {
        allowGlow = false;

        //Fade In Glow
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 5f;
            mat.SetColor("_EmissionColor", Color.Lerp(originalLumin, emissiveLumin, time));
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        //Fade Out Glow
        float time2 = 0f;
        while (time2 < 1f)
        {
            time2 += Time.deltaTime;
            mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), originalLumin, time2));
            //Unity fades out emissiveness too quickly, so the below line slows it down
            if (mat.GetColor("_EmissionColor").g > .05f ) { yield return new WaitForSeconds(.075f); }
            //Below .05f green value in the emissiveness, further lerping isn't visible. So end the coroutine early with the below line.
            else { yield break; }
        }
        allowGlow = true;
    }
}
