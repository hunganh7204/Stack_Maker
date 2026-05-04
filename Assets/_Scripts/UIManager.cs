using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingPanel;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI levelText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.text = "LEVEL " + level;
        }
    }

    //WinUI
    public void ShowWinUI()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void HideWinUI()
    {
        if (winPanel != null) winPanel.SetActive(false);
    }


    public void OnClickReplay()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }

    public void OnClickNextLevel()
    {
        LevelManager.Instance.NextLevel();
    }

    public void OnClickMenu()
    {
        ShowMainMenu();
        winPanel.SetActive(false);
    }

    //MainMenuUI
    public void ShowMainMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
    }
    public void HideMainMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    public void OnClickNewGame()
    {
        PlayerPrefs.SetInt("SavedLevel", 1);
        PlayerPrefs.Save();

        HideMainMenu();
        LevelManager.Instance.LoadLevel(1);
    }
    public void OnClickContinue()
    {
        int savedLevel = PlayerPrefs.GetInt("SavedLevel", 1);
        HideMainMenu();
        LevelManager.Instance.LoadLevel(savedLevel);
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }

    //SettingUI
    public void ShowSetting()
    {
        if (settingPanel != null) settingPanel.SetActive(true);
    }
    public void HideSetting()
    {
        if (settingPanel != null) settingPanel.SetActive(false);
    }
    public void OnClickSettingRetry()
    {
        HideSetting();
        OnClickReplay();    
    }
    public void OnClickSettingMenu()
    {
        HideSetting();
        ShowMainMenu();
    }
}

