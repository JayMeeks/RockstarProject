using System;using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GraphicalSettingsManager
{
    public static void ApplyGraphicsSettings(GraphicsInfo settings)
    {
        SetFullscreenMode(settings.fullScreenMode);
        SetGameResolution(settings.resolutionX, settings.resolutionY, settings.fullScreenMode);
        SetUseVsync(settings.useVSync);
    }

    private static void SetFullscreenMode(FullScreenMode mode)
    {
        Screen.fullScreenMode = mode;
        Screen.fullScreen = GraphicsDict.fullScreenBool[mode];
    }

    private static void SetUseVsync(bool useVsync)
    {
        QualitySettings.vSyncCount = useVsync ? 1 : 0;
    }
  
    private static void SetGameResolution(int resolutionX, int resolutionY, FullScreenMode mode)
    {
        Screen.SetResolution(resolutionX, resolutionY, GraphicsDict.fullScreenBool[mode]);
    }
}



//Static class that holds data for graphics settings
//Menu dropdowns load their values from these dictionaries
//If new settings are needed, add them to the dictionary
//Reverse dictionary is so that the correct visual for the dropdown can be easily found
public static class GraphicsDict
{
    //Dictionary for possible fullscreen modes
    //Forward and inverse dictionary
    public static readonly Dictionary<int, FullScreenMode> fullScreenmode =
        new Dictionary<int, FullScreenMode>
        {
            { 0, FullScreenMode.Windowed },
            { 1, FullScreenMode.FullScreenWindow },
            { 2, FullScreenMode.ExclusiveFullScreen }
        };
    public static readonly Dictionary<FullScreenMode, int> fullScreenmodeInv = fullScreenmode.ToDictionary((i) => i.Value, (i) => i.Key);


    //Dictionary for whether the game should be fullscreen or not, depending on fullscreen mode
    public static readonly Dictionary<FullScreenMode, bool> fullScreenBool =
        new Dictionary<FullScreenMode, bool>
        {
            { FullScreenMode.Windowed, false },
            { FullScreenMode.FullScreenWindow, true },
            { FullScreenMode.ExclusiveFullScreen, true }
        };
        
        
    //Dictionary for possible resolutions
    //Forward and inverse dictionary
    public static readonly Dictionary<int, Vector2Int> resolution =
        new Dictionary<int, Vector2Int>
        {
            { 0, new Vector2Int(1920, 1080) },
            { 1, new Vector2Int(1280, 720) },
        };
        
    public static readonly Dictionary<Vector2Int, int> resolutionInv = resolution.ToDictionary((i) => i.Value, (i) => i.Key);
        

}


//Class for saving graphics settings
[System.Serializable]
public class GraphicsInfo
{
    public int resolutionX, resolutionY;
    public FullScreenMode fullScreenMode;
    public bool useVSync;

    public static GraphicsInfo CreateDefaultSettings()
    {
        return new GraphicsInfo()
        {
            resolutionX = 1920,
            resolutionY = 1080,
            fullScreenMode = FullScreenMode.FullScreenWindow,
            useVSync = false,
        };

    }
    
    
    
}
