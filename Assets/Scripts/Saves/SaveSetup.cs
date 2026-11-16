using System.IO;
using UnityEngine;

public class SaveSetup : MonoBehaviour
{
    private static SaveSetup _instance;

    public static SaveSetup Instance
    {
        get { return _instance; }
    }





    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        CheckGraphicsSettingFileExists();
        



    }

    //If no save file exists, creates a default profile
    private void CheckGraphicsSettingFileExists()
    {
        if(!File.Exists(SaveSystem.GetGraphicsInfoFilePath()))
        {
            SaveSystem.SaveGraphicsInfo(GraphicsInfo.CreateDefaultSettings());
        }
    }
}
