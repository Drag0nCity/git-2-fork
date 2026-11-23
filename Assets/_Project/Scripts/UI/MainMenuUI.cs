
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OpenLevelSelect);
        else
            Debug.LogError("MainMenuUI: Play Button is not assigned!");

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
        else
            Debug.LogError("MainMenuUI: Exit Button is not assigned!");
    }

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(OpenLevelSelect);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(ExitGame);
    }

    private void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    private void ExitGame()
    {
        Debug.Log("Exiting Puzzle World...");
        Application.Quit();
    }
}
