
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(LevelStrategyManager))]
public class LevelLoader : MonoBehaviour
{
    [Header("Level Configurations")]
    [SerializeField] private LevelData[] levels;

    [Header("Level Spawn")]
    [SerializeField] private Transform levelParent;

    private LevelStrategyManager strategyManager;
    private SaveService saveService;
    private GameObject currentLevelInstance;

    public LevelData CurrentLevel { get; private set; }

    [Inject]
    private void Construct(SaveService service)
    {
        saveService = service;
    }

    private void Awake()
    {
        strategyManager = GetComponent<LevelStrategyManager>();
        LoadCurrentLevel();
    }

    private void Start()
    {
        if (CurrentLevel == null)
            return;

        strategyManager.ConfigureFromLevel(CurrentLevel);

        ApplyEnvironment();
        SpawnLevelPrefab();
    }

    public void LoadCurrentLevel()
    {
        if (saveService == null)
        {
            Debug.LogError(
                "LevelLoader: SaveService was not injected!"
            );
            return;
        }

        int selectedLevel = saveService.Data.currentLevel;

        foreach (LevelData level in levels)
        {
            if (level != null && level.levelNumber == selectedLevel)
            {
                CurrentLevel = level;

                Debug.Log("Loaded level: " + level.levelName);
                return;
            }
        }

        Debug.LogError(
            "LevelLoader: Level " + selectedLevel +
            " was not found in the assigned LevelData assets!"
        );
    }

    private void SpawnLevelPrefab()
    {
        if (CurrentLevel.levelPrefab == null)
        {
            Debug.LogError(
                "LevelLoader: Prefab is not assigned for " +
                CurrentLevel.levelName
            );
            return;
        }

        currentLevelInstance = Instantiate(
            CurrentLevel.levelPrefab,
            Vector3.zero,
            Quaternion.identity,
            levelParent
        );

        currentLevelInstance.name = CurrentLevel.levelPrefab.name;

        Debug.Log(
            "Level prefab spawned: " + currentLevelInstance.name
        );
    }

    private void ApplyEnvironment()
    {
        if (CurrentLevel.skyboxMaterial == null)
        {
            Debug.LogWarning(
                "LevelLoader: Skybox Material is not assigned for " +
                CurrentLevel.levelName
            );
            return;
        }

        RenderSettings.skybox = CurrentLevel.skyboxMaterial;

        DynamicGI.UpdateEnvironment();

        Debug.Log(
            "Skybox applied: " + CurrentLevel.skyboxMaterial.name
        );
    }

    public void RestartCurrentLevel()
    {
        Debug.Log("Restarting current level...");

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
