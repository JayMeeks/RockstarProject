using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{

    
    
    //Saves graphics settings info to the persistent data path (AppData/LocalLow/DefaultCompany/)
    //Saves to "GraphicsInfo.txt" in json format
    //No encryption
    public static void SaveGraphicsInfo(GraphicsInfo settings)
    {
        string filePath = GetGraphicsInfoFilePath();
        
        string json = JsonUtility.ToJson(settings);
        
        File.WriteAllText(filePath, json);
    }


    //TODO: in future: Probably cache this somewhere? not sure if should be cached here
    //Not much data as of yet so i don't think its a huge performance hit to load this but it feels kinda bad 
    //Loads saved settings from disk
    public static GraphicsInfo LoadGraphicsInfo()
    {
        return JsonUtility.FromJson<GraphicsInfo>(File.ReadAllText(GetGraphicsInfoFilePath()));
    }

    public static string GetGraphicsInfoFilePath()
    {
       return Application.persistentDataPath + "/GraphicsInfo.txt";
    }
    
    
    
    
    
    
}
