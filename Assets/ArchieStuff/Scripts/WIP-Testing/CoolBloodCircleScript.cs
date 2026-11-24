using System;
using System.Collections;
using UnityEngine;

public class CoolBloodCircleScript : MonoBehaviour
{
    [SerializeField] private GameObject menuPrefab;
    
    private GameObject menu;
    private float scale = 0.3f;
    
    private bool coroutineRunning = false;
    
  


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !coroutineRunning)
        {
            menu = Instantiate(menuPrefab, transform.position, transform.rotation);
            StartCoroutine(GrowMenu(1.5f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !coroutineRunning)
        {
            StartCoroutine(ShrinkMenu(1.5f));
        }
    }

    private IEnumerator GrowMenu(float duration)
    {
        coroutineRunning = true;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            menu.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(scale,scale,scale), t / duration);
            menu.transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x,transform.position.y + 2f,transform.position.z), t / duration);
            yield return null;
        }
        coroutineRunning = false;
    }
    
    private IEnumerator ShrinkMenu(float duration)
    {
        coroutineRunning = true;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            menu.transform.localScale = Vector3.Lerp(new Vector3(scale,scale,scale), Vector3.zero, t / duration);
            menu.transform.position = Vector3.Lerp(new Vector3(transform.position.x,transform.position.y + 2f,transform.position.z),transform.position, t / duration);
            yield return null;
        }
        coroutineRunning = false;
        Destroy(menu.gameObject);
    }
}
