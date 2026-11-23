using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Zenject;

public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private string sceneName = "GameScene";

    [Header("UI")]
    [SerializeField] private Button levelButton;
    [SerializeField] private TMP_Text buttonText;

    private SaveService saveService;

    [Inject]
    private void Construct(SaveService service)
    {
        saveService = service;
    }

    private void Start()
    {
        if (levelButton == null)
            levelButton = GetComponent<Button>();

        if (levelButton != null)
            levelButton.onClick.AddListener(LoadLevel);

        RefreshButtonState();
    }

    private void OnDestroy()
    {
        if (levelButton != null)
            levelButton.onClick.RemoveListener(LoadLevel);
    }

    public void RefreshButtonState()
    {
        if (saveService == null)
        {
            Debug.LogError("LevelButton: SaveService was not injected!", this);
            return;
        }

        bool unlocked = saveService.IsLevelUnlocked(levelNumber);

        if (levelButton != null)
            levelButton.interactable = unlocked;

        if (buttonText != null)
        {
            buttonText.text = unlocked
                ? "LEVEL " + levelNumber
                : "LEVEL " + levelNumber + " (LOCKED)";
        }
    }

    private void LoadLevel()
    {
        if (saveService == null)
            return;

        if (!saveService.IsLevelUnlocked(levelNumber))
            return;

        saveService.SetCurrentLevel(levelNumber);

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
