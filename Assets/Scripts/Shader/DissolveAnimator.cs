using System;
using System.Collections;
using UnityEngine;

public class DissolveAnimator : MonoBehaviour
{
    public Material dissolveMaterial;


   


    public void PlayAnimation()
    {
       StartCoroutine(Dissolve());
        
        
    }

    private IEnumerator Dissolve()
    {
        float alpha = 0.0f;

        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime;
            dissolveMaterial.SetFloat("_threshold", alpha);
            yield return null;
        }
    }
   
}
