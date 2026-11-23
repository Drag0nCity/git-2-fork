
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;

public class VictoryUI : MonoBehaviour
{
    [Header("Victory Panel")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button menuButton;

    [Header("Defeat Panel")]
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private Button defeatMenuButton;

    private SaveService saveService;
    private bool progressSaved;

    [Inject]
    private void Construct(SaveService service)
    {
        saveService = service;
    }

    private void Awake()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnLevelCompleted += ShowVictory;
        GameEvents.OnLevelFailed += ShowDefeat;

        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);

        if (defeatMenuButton != null)
            defeatMenuButton.onClick.AddListener(ReturnToMenu);
    }

    private void OnDisable()
    {
        GameEvents.OnLevelCompleted -= ShowVictory;
        GameEvents.OnLevelFailed -= ShowDefeat;

        if (menuButton != null)
            menuButton.onClick.RemoveListener(ReturnToMenu);

        if (defeatMenuButton != null)
            defeatMenuButton.onClick.RemoveListener(ReturnToMenu);
    }

    private void ShowVictory()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (!progressSaved)
        {
            if (saveService != null)
            {
                int completedLevel = saveService.Data.currentLevel;
                saveService.CompleteLevel(completedLevel);
                progressSaved = true;

                Debug.Log("Level " + completedLevel + " progress saved!");
            }
            else
            {
                Debug.LogError("VictoryUI: SaveService was not injected!");
            }
        }

        Time.timeScale = 0f;
    }

    private void ShowDefeat()
    {
        if (defeatPanel != null)
            defeatPanel.SetActive(true);
        else
            Debug.LogError("VictoryUI: Defeat Panel is not assigned!");

        Time.timeScale = 0f;
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

