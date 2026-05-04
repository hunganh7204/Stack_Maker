using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SaveLevel(LevelData levelData, string fileName)
    {
        string folderPath = Application.dataPath + "/Resources/Level";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!fileName.EndsWith(".json")) fileName += ".json";

        string fullPath = Path.Combine(folderPath, fileName);
        string json = JsonUtility.ToJson(levelData, true);

        File.WriteAllText(fullPath, json);

        UnityEditor.AssetDatabase.Refresh();
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
            Debug.LogError("❌ Không tìm thấy map: " + fileName + " trong thư mục Resources/Level!");
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
