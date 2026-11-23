using UnityEngine;

[CreateAssetMenu(
    fileName = "NewLevel",
    menuName = "Puzzle World/Level Data"
)]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public int levelNumber = 1;
    public string levelName = "First Steps";

    [Header("Level Prefab")]
    public GameObject levelPrefab;

    [Header("Board Settings")]
    [Min(1)] public int gridWidth = 7;
    [Min(1)] public int gridHeight = 7;
    [Min(0.1f)] public float cellSize = 1f;

    [Header("Move Settings")]
    public bool useMoveLimit;
    [Min(1)] public int maxMoves = 10;

    [Header("Environment")]
    public Material skyboxMaterial;
}
