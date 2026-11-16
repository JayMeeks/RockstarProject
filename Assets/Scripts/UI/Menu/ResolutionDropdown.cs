using TMPro;
using UnityEngine;

public class ResolutionDropdown : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private int resolutionCount;
  
    private void Start()
    {
        InitDropdown();
        LoadDropdownValue();
        
     
        //Update visual
        dropdown.RefreshShownValue();
    }


    private void InitDropdown()
    {
        //Get dropdown
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.options.Clear();
        
        //Get number of options
        resolutionCount = GraphicsDict.resolution.Count;
        
        
        //Add each option to the dropdown
        //Loaded from dictionary
        for (int i = 0; i < resolutionCount; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(GraphicsDict.resolution[i].x + "x" + GraphicsDict.resolution[i].y));
        }
    }

    private void LoadDropdownValue()
    {
        //Load saved settings 
        GraphicsInfo settings = SaveSystem.LoadGraphicsInfo();
     
        //Check there is a resolution actually saved
        if (settings.resolutionX != 0)
        {
            //Set the current selected value without triggering any events
            dropdown.SetValueWithoutNotify(GraphicsDict.resolutionInv[new Vector2Int(settings.resolutionX, settings.resolutionY)]);
        }
    }


 
}
