using System;
using System.Collections;
using UnityEngine;

public class TestIntroManager : MonoBehaviour
{
    [SerializeField] private GameObject gate;
    private Animator gateanim;
   
    private void Awake()
    {
        RenderSettings.fog = true;
        RenderSettings.fogDensity = 0.3f;
        
        gateanim = gate.GetComponent<Animator>();
        

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOutFog());
            gateanim.Play("GateOpen",0,0);
        }
    }

    private IEnumerator FadeOutFog()
    {
        float t = 0;
     

        while (t < 2.0f)
        {
            t += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(0.4f,0,t/2.0f);
            yield return null;
        }
        RenderSettings.fog = false;
    }
}
