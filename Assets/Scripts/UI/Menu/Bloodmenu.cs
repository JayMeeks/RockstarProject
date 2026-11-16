using System;
using UnityEngine;

public class Bloodmenu : MonoBehaviour
{
  //Holder for settings that are applied from the apply button
  //Serialized for testing only to expose to editor and ensure values are correct
  [SerializeField] private GraphicsInfo localGraphicsSettings = new GraphicsInfo();

  private void Start()
  {
    //Load graphics settings from disk
    localGraphicsSettings = SaveSystem.LoadGraphicsInfo();
  }

  //Called by button event
  //Changes local setting to the changed setting
  public void ChangeResolution(int resIndex) //Index is the value in the dropdown list, fetched from dictionary of possible values
  {
    GraphicsDict.resolution.TryGetValue(resIndex, out Vector2Int res);
    localGraphicsSettings.resolutionX = (int)res.x;
    localGraphicsSettings.resolutionY = (int)res.y;
  }

  //Called by button event
  //Changes local setting to the changed setting
  public void ChangeFullscreenMode(int modeIndex) //Index is the value in the dropdown list, fetched from dictionary of possible values
  {
    GraphicsDict.fullScreenmode.TryGetValue(modeIndex, out FullScreenMode mode);
    localGraphicsSettings.fullScreenMode = mode;
  }

  //Called by button event
  //Changes local setting to the changed setting
  public void ChangeUseVsync(bool useVsync) //Value from button toggle
  {
    localGraphicsSettings.useVSync = useVsync;
  }

  //Saves locally stored changes to disk and applies settings
  public void ApplyGraphicsSettings()
  {
    SaveSystem.SaveGraphicsInfo(localGraphicsSettings);
    GraphicalSettingsManager.ApplyGraphicsSettings(localGraphicsSettings);
  }
}
