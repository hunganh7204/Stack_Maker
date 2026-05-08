using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI levelText;

    private int currentLevel = 0;
    public int CurrentLevel => currentLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.StartLoadingSequence();
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;
        PlayerPrefs.SetInt("SavedLevel", currentLevel);
        PlayerPrefs.Save();
        UIManager.Instance.UpdateLevelText(currentLevel);
        UIManager.Instance.HideWinUI();
        LevelData data = DataManager.Instance.LoadLevel("Level_" + level);

        if(data != null)
        {
            levelGenerator.BuildLevelFromData(data);
            OnInit();
        }
        else
        {
            Debug.LogError("Failed to load level data for Level " + level);
        }
    }

    public void OnInit()
    {
        Vector3 startPos = levelGenerator.GetStartPosition();
        player.OnInit(startPos);
        Invoke(nameof(OnPlay), 1f);
    }

    public void OnPlay()
    {
        player.OnPlay();
    }

    public void NextLevel()
    {
        currentLevel++;
        LoadLevel(currentLevel);
    }
}
