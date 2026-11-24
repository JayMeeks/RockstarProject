using TMPro;
using UnityEngine;

public class FullscreenModeDropdown : MonoBehaviour
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
        resolutionCount = GraphicsDict.fullScreenmode.Count;
        
        
        //Add each option to the dropdown
        //Loaded from dictionary
        for (int i = 0; i < resolutionCount; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(GraphicsDict.fullScreenmode[i].ToString()));
        }
    }

    private void LoadDropdownValue()
    {
        //Load saved settings 
        GraphicsInfo settings = SaveSystem.LoadGraphicsInfo();

     
        //Check there is a resolution actually saved. somehow
        //If you see this and i haven't fixed it remind me
        //TODO:
        if (true)
        {
            //Set the current selected value without triggering any events
            dropdown.SetValueWithoutNotify(GraphicsDict.fullScreenmodeInv[settings.fullScreenMode]);
        }
    }


 
}