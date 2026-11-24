using UnityEngine;
using UnityEngine.UI;

public class VSyncToggle : MonoBehaviour
{
    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        //Load saved settings 
        GraphicsInfo settings = SaveSystem.LoadGraphicsInfo();
        toggle.isOn = settings.useVSync;
    }
    
}
