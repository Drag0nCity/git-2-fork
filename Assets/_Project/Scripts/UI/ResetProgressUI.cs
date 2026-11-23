using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResetProgressUI : MonoBehaviour
{
    [Header("Confirmation Panel")]
    [SerializeField] private GameObject confirmationPanel;

    [Header("Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private SaveService saveService;

    [Inject]
    private void Construct(SaveService service)
    {
        saveService = service;
    }

    private void Awake()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        if (resetButton != null)
            resetButton.onClick.AddListener(OpenConfirmation);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmReset);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelReset);
    }

    private void OpenConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);
    }

    private void ConfirmReset()
    {
        if (saveService == null)
        {
            Debug.LogError("ResetProgressUI: SaveService was not injected!");
            return;
        }

        saveService.ResetProgress();

        Time.timeScale = 1f;

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        RefreshLevelButtons();

        Debug.Log("Game progress reset successfully!");
    }

    private void CancelReset()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    private void RefreshLevelButtons()
    {
        LevelButton[] buttons =
            FindObjectsByType<LevelButton>(FindObjectsSortMode.None);

        foreach (LevelButton button in buttons)
            button.RefreshButtonState();
    }

    private void OnDestroy()
    {
        if (resetButton != null)
            resetButton.onClick.RemoveListener(OpenConfirmation);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(ConfirmReset);

        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(CancelReset);
    }
}
