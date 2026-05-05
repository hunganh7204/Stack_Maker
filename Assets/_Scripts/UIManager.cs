using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject losePanel;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("UI Settings")]
    [SerializeField] private float loseFadeDuration = 2.5f;

    [Header("HUD Elements")]
    [SerializeField] private GameObject settingButton;

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
        if (settingButton != null) settingButton.SetActive(false);
    }

    public void HideWinUI()
    {
        if (winPanel != null) winPanel.SetActive(false);
    }


    public void OnClickReplay()
    {
        HideWinUI();
        HideLoseUI();
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }

    public void OnClickNextLevel()
    {
        LevelManager.Instance.NextLevel();
    }

    public void OnClickMenu()
    {
        HideWinUI();
        HideLoseUI();
        ShowMainMenu();
    }

    //MainMenuUI
    public void ShowMainMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (settingButton != null) settingButton.SetActive(false);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMainMenuBGM();
    }
    public void HideMainMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingButton != null) settingButton.SetActive(true);
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM(true, 1.5f);
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

    //LoseUI
    public void ShowLoseUI()
    {
        if (losePanel != null) losePanel.SetActive(true);
        if (settingButton != null) settingButton.SetActive(false);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLoseSFX();
        CanvasGroup canvasGroup = losePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = losePanel.AddComponent<CanvasGroup>();
        }

        StartCoroutine(FadeIn(canvasGroup)); 
    }
     public void HideLoseUI()
    {
        if (losePanel != null) losePanel.SetActive(false);
    }
    public void OnClickLoseMenu()
    {
        HideLoseUI();
        ShowMainMenu();
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        cg.alpha = 0f;

        cg.interactable = false;

        float elapsedTime = 0f;

        while (elapsedTime < loseFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsedTime / loseFadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
        cg.interactable = true;
    }
}

