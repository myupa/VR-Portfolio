using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeShadowRenderer : MonoBehaviour
{
    //Has to be placed on every GO that will have its rendererer fade out

    void Start()
    {
        MushroomSceneManager.RendererFade += RunFadeOutShadows;
    }

    void OnDestroy()
    {
        MushroomSceneManager.RendererFade -= RunFadeOutShadows;
    }

    public void RunFadeOutShadows()
    {
        StartCoroutine("FadeOutShadows");
    }

    public IEnumerator FadeOutShadows()
    {
        SkinnedMeshRenderer shadowtransp = gameObject.GetComponent<SkinnedMeshRenderer>();
        // shadowtransp.material.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
        //shadowtransp.material.SetFloat("_Mode", 2.0f);

        shadowtransp.material.SetFloat("_Mode", 2);
        shadowtransp.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        shadowtransp.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        shadowtransp.material.SetInt("_ZWrite", 0);
        shadowtransp.material.DisableKeyword("_ALPHATEST_ON");
        shadowtransp.material.EnableKeyword("_ALPHABLEND_ON");
        shadowtransp.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        shadowtransp.material.renderQueue = 3000;

        float time = 0f;
        var tempcolor = shadowtransp.material.color;
        while (time < 2f)
        {
            time += Time.deltaTime *.1f;
            //shadowtransp.material.color = Color.Lerp(MainSceneManager.SceneManagerInstance.startShadowColor, MainSceneManager.SceneManagerInstance.endShadowColor, shadowLerpTime);
            tempcolor.a = Mathf.Lerp(1f, 0f, time);
            shadowtransp.material.color = tempcolor;
            yield return null;
        }

    }


}
