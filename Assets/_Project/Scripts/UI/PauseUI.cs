using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseUI : MonoBehaviour
{
    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Level")]
    [SerializeField] private LevelLoader levelLoader;

    [Header("Debug Restart")]
    [SerializeField] private float restartHoldDuration = 1.5f;

    private InputAction pauseAction;
    private InputAction restartAction;

    private bool isPaused;
    private bool restartKeyHeld;
    private bool restartTriggered;

    private float restartHoldStartTime;

    private void Awake()
    {
        if (pausePanel == null)
        {
            Debug.LogError("PauseUI: Pause Panel is not assigned!", this);
            return;
        }

        pausePanel.SetActive(false);
        isPaused = false;

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);
    }

    private void OnEnable()
    {
        // TAB и ESC — переключение паузы.
        pauseAction = new InputAction(
            "Pause",
            InputActionType.Button
        );

        pauseAction.AddBinding("<Keyboard>/escape");
        pauseAction.AddBinding("<Keyboard>/tab");

        pauseAction.performed += OnPausePressed;
        pauseAction.Enable();

        // Удержание клавиши ` / ~.
        restartAction = new InputAction(
            "DebugRestart",
            InputActionType.Button,
            "<Keyboard>/backquote"
        );

        restartAction.started += OnRestartKeyPressed;
        restartAction.canceled += OnRestartKeyReleased;
        restartAction.Enable();
    }

    private void Update()
    {
        if (!restartKeyHeld || restartTriggered)
            return;

        float heldTime = Time.unscaledTime - restartHoldStartTime;

        if (heldTime >= restartHoldDuration)
        {
            restartTriggered = true;
            Debug.Log("Debug restart triggered!");
            RestartLevel();
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    private void OnRestartKeyPressed(InputAction.CallbackContext context)
    {
        restartKeyHeld = true;
        restartTriggered = false;
        restartHoldStartTime = Time.unscaledTime;
    }

    private void OnRestartKeyReleased(InputAction.CallbackContext context)
    {
        restartKeyHeld = false;
        restartTriggered = false;
    }

    private void TogglePause()
    {
        if (pausePanel == null)
            return;

        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;

        if (levelLoader != null)
        {
            levelLoader.RestartCurrentLevel();
        }
        else
        {
            Debug.LogError("PauseUI: LevelLoader is not assigned!", this);
        }
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePressed;
            pauseAction.Disable();
            pauseAction.Dispose();
            pauseAction = null;
        }

        if (restartAction != null)
        {
            restartAction.started -= OnRestartKeyPressed;
            restartAction.canceled -= OnRestartKeyReleased;
            restartAction.Disable();
            restartAction.Dispose();
            restartAction = null;
        }

        restartKeyHeld = false;
        restartTriggered = false;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartLevel);

        if (menuButton != null)
            menuButton.onClick.RemoveListener(ReturnToMenu);
    }
}
