using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    //Holder for settings applied from apply button
    //Serialized for testing only to expose to editor and ensure values are correct
    [SerializeField] private GraphicsInfo localGraphicsSettings = new GraphicsInfo();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Load graphics settings from disk
        localGraphicsSettings = SaveSystem.LoadGraphicsInfo();
    }

    //Called by button event
    //Changes local setting to the changed setting
    public void ChangeResolution(int resIndex) //Index is the value in the dropdown list, fetched from dictionary of possible values
    {
        GraphicsDict.resolution.TryGetValue(resIndex, out Vector2Int _res);
        localGraphicsSettings.resolutionX = (int)_res.x;
        localGraphicsSettings.resolutionY = (int)_res.y;
    }

    //Called by button event
    //Changes local setting to the changed setting
    public void ChangeFullscreenMode(int modeIndex)
    {
        GraphicsDict.fullScreenmode.TryGetValue(modeIndex, out FullScreenMode _mode); //Index is the value in the dropdown list, fetched from dictionary of possible values
        localGraphicsSettings.fullScreenMode = _mode;
    }

    //Called by button event
    //Changes local setting to the changed setting
    public void ChangeUseVsync(bool useVsync) 
    {
        localGraphicsSettings.useVSync = useVsync; //Value from button toggle
    }

    //Saves locally stored changes to disk and applies settings
    public void ApplyGraphicsSettings()
    {
        SaveSystem.SaveGraphicsInfo(localGraphicsSettings);
        GraphicalSettingsManager.ApplyGraphicsSettings(localGraphicsSettings);
    }
}
