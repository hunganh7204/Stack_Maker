using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public int savedLevel = 1;
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public GameData gameData;

    const string GameDataKey = "GameData";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGameData()
    {
        if (PlayerPrefs.HasKey(GameDataKey))
        {
            string json = PlayerPrefs.GetString(GameDataKey);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData();
        }
    }

    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString(GameDataKey, json);
        PlayerPrefs.Save();
    }

    public void SaveCurrentLevel (int level)
    {
        gameData.savedLevel = level;
        SaveGameData();
    }

    public void SaveLevel(LevelData levelData, string fileName)
    {
        string folderPath = Application.dataPath + "/Resources/Level";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!fileName.ToLower().EndsWith(".json")) fileName += ".json";

        string fullPath = Path.Combine(folderPath, fileName);
        string json = JsonUtility.ToJson(levelData, true);

        File.WriteAllText(fullPath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public LevelData LoadLevel(string fileName)
    {
        string cleanName = fileName.Replace(".json", "");
        string resourcePath = "Level/" + cleanName;

        TextAsset resourceFile = Resources.Load<TextAsset>(resourcePath);

        if (resourceFile != null)
        {
            return JsonUtility.FromJson<LevelData>(resourceFile.text);
        }
        else
        {
            return null;
        }
    }

    public string GetNextLevelName()
    {
        int maxLevel = 0;

        TextAsset[] files = Resources.LoadAll<TextAsset>("Level");

        foreach (TextAsset file in files)
        {
            string[] parts = file.name.Split('_');
            if (parts.Length > 1 && int.TryParse(parts[1], out int num))
            {
                if (num > maxLevel) maxLevel = num;
            }
        }

        return "Level_" + (maxLevel + 1);
    }
}
