using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/Levels/";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SaveLevel(LevelData levelData, string fileName)
    {
        string json = JsonUtility.ToJson(levelData, true);

        string fullPath = savePath + fileName + ".json";
        File.WriteAllText(fullPath, json);
    }

    public LevelData LoadLevel(string fileName)
    {
        string fullPath = savePath + fileName + ".json";

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            LevelData data = JsonUtility.FromJson<LevelData>(json);
            return data;
        }
        else
        {
            return null;
        }
    }

    public string GetNextLevelName()
    {
        int maxLevel = 0;
        if (Directory.Exists(savePath))
        {
            string[] files = Directory.GetFiles(savePath, "Level_*.json");
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string[] parts = fileName.Split('_');
                if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                {
                    if (num > maxLevel) maxLevel = num;
                }
            }
        }
        return "Level_" + (maxLevel + 1);
    }
}
