using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = string.Empty;
    private string dataFileName = string.Empty;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }
    
    public GameData LoadData()
    {
        // Combine to account for different OS using different path seperators
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            // Load serialized data from JSON file
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Deserialize data from JSON back to C# file
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch (Exception e)
            {
                Debug.Log("Error occured when loading data from file" + fullPath + "\n" + e);
            }

            
        }
        return loadedData;
    }

    public void SaveData(GameData data)
    {
        // Combine to account for different OS using different path seperators
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // Create directory file if does not exist and be written
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data into JSON file
            string dataToStore = JsonUtility.ToJson(data, true);

            // Write serialized data to file
            using (FileStream stream =  new FileStream(fullPath, FileMode.Create)) 
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.Log("Error occured when saving data to file" + fullPath + "\n" + e);
        }
    }
}
