using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER_PLAYER = Application.dataPath + "/Saves/Player/";
    private static readonly string SAVE_FOLDER_LEVELS = Application.dataPath + "/Saves/Levels/";
    private static readonly string SAVE_SKINS_DATA = Application.dataPath + "/Saves/Skins/";
    private static readonly string SAVE_SPIRITS = Application.dataPath + "/Saves/Spirits/";

    public static bool justReset = false;

    public static void Init()
    {
        if (!Directory.Exists(SAVE_FOLDER_PLAYER))
        {
            Directory.CreateDirectory(SAVE_FOLDER_PLAYER);
        }

        if (!Directory.Exists(SAVE_FOLDER_LEVELS))
        {
            Directory.CreateDirectory(SAVE_FOLDER_LEVELS);
        }

        if (!Directory.Exists(SAVE_SKINS_DATA))
        {
            Directory.CreateDirectory(SAVE_SKINS_DATA);
        }

        if (!Directory.Exists(SAVE_SPIRITS))
        {
            Directory.CreateDirectory(SAVE_SPIRITS);
        }
    }

    public static void SavePlayer(string saveString)
    {
        int saveNumber = 1;
        while (File.Exists("save_" + saveNumber + ".txt"))
        {
            saveNumber++;
        }
        File.WriteAllText(SAVE_FOLDER_PLAYER + "save_" + saveNumber + ".txt", saveString);
    }

    public static void SaveLevelData(string saveString, int id)
    {
        File.WriteAllText(SAVE_FOLDER_LEVELS + "level_" + id + ".txt", saveString); 
    }

    public static void SaveSkinData(string saveString, int id)
    {
        File.WriteAllText(SAVE_SKINS_DATA + "skin_" + id + ".txt", saveString);
    }

    public static void SaveSpiritData(string saveString)
    { 
        File.WriteAllText(SAVE_SPIRITS + "spirits.txt", saveString);
    }

    public static string LoadSpiritData()
    {
        if (File.Exists(SAVE_SPIRITS + "spirits.txt"))
        {
            return File.ReadAllText(SAVE_SPIRITS + "spirits.txt");
        }
        else
            return null;
    }

    public static string LoadLevelData(int id)
    {
        if (File.Exists(SAVE_FOLDER_LEVELS + "level_" + id + ".txt"))
        { 
            return File.ReadAllText(SAVE_FOLDER_LEVELS + "level_" + id + ".txt");
        }
        else
            return null; 
    }

    public static string LoadSkinData(int id)
    {
        if (File.Exists(SAVE_SKINS_DATA + "skin_" + id + ".txt"))
        {
            return File.ReadAllText(SAVE_SKINS_DATA + "skin_" + id + ".txt");
        }
        else
            return null;
    }


    public static string LoadPlayer()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER_PLAYER);
        FileInfo[] saveFiles = directoryInfo.GetFiles("*.txt");
        FileInfo mostRecentFile = null;
        foreach (FileInfo fileInfo in saveFiles)
        {
            if (mostRecentFile == null)
            {
                mostRecentFile = fileInfo;
            }
            else
            {
                if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
                {
                    mostRecentFile = fileInfo;
                }
            }
        }

        if (mostRecentFile != null)
        {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        }
        else
            return null; 
    }
    public static void DeleteSpiritData()
    {
        DeleteSaves(SAVE_SPIRITS);
    }

    public static void ResetProgress()
    {
        DeleteSaves(SAVE_FOLDER_PLAYER);
        DeleteSaves(SAVE_FOLDER_LEVELS);
        DeleteSaves(SAVE_SKINS_DATA);
        DeleteSpiritData();
        justReset = true; 
    }

    public static void DeleteSaves(string path) //deletes all level and player progress 
    {
        if (Directory.Exists(path))
        {
            var saveFiles = Directory.GetFiles(path);

            for (int i = 0; i < saveFiles.Length; i++)
            {
                    File.Delete(saveFiles[i]);
            }

            Directory.Delete(path);
            Directory.CreateDirectory(path);
        }
    }
}
