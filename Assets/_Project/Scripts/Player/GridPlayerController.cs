using UnityEngine;
using UnityEngine.InputSystem;

public class GridPlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Grid")]
    [SerializeField] private int gridWidth = 7;
    [SerializeField] private int gridHeight = 7;
    [SerializeField] private float cellSize = 1f;

    [Header("Level")]
    [SerializeField] private LevelLoader levelLoader;

    [Header("Strategy")]
    [SerializeField] private LevelStrategyManager strategyManager;

    private InputAction moveAction;
    private InputAction undoAction;

    private Vector2Int gridPosition;
    private CommandHistory commandHistory;

    private int moveCount;
    private bool isLevelCompleted;

    private readonly Collider[] hitBuffer = new Collider[16];

    private void Awake()
    {
        commandHistory = new CommandHistory();

        if (strategyManager == null)
            strategyManager = FindFirstObjectByType<LevelStrategyManager>();

        if (levelLoader == null)
            levelLoader = FindFirstObjectByType<LevelLoader>();

        if (levelLoader != null && levelLoader.CurrentLevel != null)
        {
            LevelData data = levelLoader.CurrentLevel;

            gridWidth = data.gridWidth;
            gridHeight = data.gridHeight;
            cellSize = data.cellSize;
        }
        else
        {
            Debug.LogWarning(
                "GridPlayerController: LevelData not found. Using default grid settings."
            );
        }

        gridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / cellSize),
            Mathf.RoundToInt(transform.position.z / cellSize)
        );

        if (strategyManager == null)
        {
            Debug.LogError(
                "GridPlayerController: LevelStrategyManager is not assigned!",
                this
            );
        }
    }

    private void Start()
    {
        Physics.SyncTransforms();
        RefreshSwitchState();
    }

    private void OnEnable()
    {
        GameEvents.OnLevelFailed += HandleLevelFailed;

        if (inputActions == null)
        {
            Debug.LogError(
                "GridPlayerController: Input Actions is not assigned!",
                this
            );
            return;
        }

        moveAction = inputActions.FindAction("Gameplay/Move");
        undoAction = inputActions.FindAction("Gameplay/Undo");

        if (moveAction == null || undoAction == null)
        {
            Debug.LogError("Move or Undo action was not found!", this);
            return;
        }

        moveAction.performed += OnMove;
        undoAction.performed += OnUndo;

        moveAction.Enable();
        undoAction.Enable();
    }

    private void OnDisable()
    {
        GameEvents.OnLevelFailed -= HandleLevelFailed;

        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.Disable();
        }

        if (undoAction != null)
        {
            undoAction.performed -= OnUndo;
            undoAction.Disable();
        }
    }

    private void HandleLevelFailed()
    {
        isLevelCompleted = true;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (isLevelCompleted)
            return;

        Vector2 input = context.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            TryMove(input.x > 0 ? Vector2Int.right : Vector2Int.left);
        }
        else if (Mathf.Abs(input.y) > 0)
        {
            TryMove(input.y > 0 ? Vector2Int.up : Vector2Int.down);
        }
    }

    private void TryMove(Vector2Int direction)
    {
        if (isLevelCompleted)
            return;

        Vector3 previousPosition = transform.position;
        Vector2Int nextPosition = gridPosition + direction;

        if (!IsInsideGrid(nextPosition))
            return;

        if (strategyManager != null &&
            !strategyManager.CanMove(nextPosition.x, nextPosition.y))
        {
            Debug.Log("Move limit reached!");
            return;
        }

        Vector3 nextWorldPosition = GridToWorld(nextPosition);
        GridBlock block = FindBlockAt(nextWorldPosition);

        if (block != null)
        {
            if (!block.IsPushable)
                return;

            Vector2Int blockNextPosition = nextPosition + direction;
            Vector3 blockNextWorldPosition;

            switch (block.BlockType)
            {
                case GridBlockType.Normal:
                    if (!IsInsideGrid(blockNextPosition))
                        return;

                    if (FindBlockAt(GridToWorld(blockNextPosition)) != null)
                        return;

                    blockNextWorldPosition = GridToWorld(blockNextPosition);
                    break;

                case GridBlockType.Light:
                    if (!TryGetLightBlockDestination(
                        blockNextPosition,
                        direction,
                        block.transform.position.y,
                        out blockNextWorldPosition))
                    {
                        return;
                    }
                    break;

                case GridBlockType.Heavy:
                    Vector2Int secondCell = blockNextPosition + direction;

                    if (!IsInsideGrid(blockNextPosition) ||
                        !IsInsideGrid(secondCell))
                    {
                        return;
                    }

                    if (FindBlockAt(GridToWorld(blockNextPosition)) != null ||
                        FindBlockAt(GridToWorld(secondCell)) != null)
                    {
                        return;
                    }

                    blockNextWorldPosition = GridToWorld(blockNextPosition);
                    blockNextWorldPosition.y = block.transform.position.y;
                    break;

                default:
                    return;
            }

            blockNextWorldPosition.y = block.transform.position.y;

            MoveCommand pushCommand = new MoveCommand(
                transform,
                nextWorldPosition,
                block.transform,
                blockNextWorldPosition
            );

            commandHistory.ExecuteCommand(pushCommand);
        }
        else
        {
            MoveCommand moveCommand = new MoveCommand(
                transform,
                nextWorldPosition
            );

            commandHistory.ExecuteCommand(moveCommand);
        }

        gridPosition = nextPosition;
        moveCount++;

        strategyManager?.RegisterMove();

        GameEvents.RaiseMoveCountChanged(moveCount);

        Physics.SyncTransforms();

        RefreshSwitchState();
        CheckGoal();

        if (!isLevelCompleted)
        {
            GameEvents.RaisePlayerMoved(
                previousPosition,
                transform.position
            );
        }

        if (!isLevelCompleted &&
            strategyManager != null &&
            strategyManager.HasRunOutOfMoves)
        {
            isLevelCompleted = true;

            Debug.Log("Level failed! No moves remaining.");

            GameEvents.RaiseLevelFailed();
        }
    }

    private bool TryGetLightBlockDestination(
        Vector2Int firstPosition,
        Vector2Int direction,
        float blockY,
        out Vector3 destination)
    {
        destination = Vector3.zero;

        if (!IsInsideGrid(firstPosition))
            return false;

        if (FindBlockAt(GridToWorld(firstPosition)) != null)
            return false;

        Vector2Int lastFreePosition = firstPosition;
        Vector2Int checkPosition = firstPosition + direction;

        while (IsInsideGrid(checkPosition))
        {
            if (FindBlockAt(GridToWorld(checkPosition)) != null)
                break;

            lastFreePosition = checkPosition;
            checkPosition += direction;
        }

        destination = GridToWorld(lastFreePosition);
        destination.y = blockY;

        return true;
    }

    private void OnUndo(InputAction.CallbackContext context)
    {
        if (isLevelCompleted || commandHistory.Count == 0)
            return;

        Vector3 previousPosition = transform.position;

        commandHistory.UndoLastCommand();

        gridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / cellSize),
            Mathf.RoundToInt(transform.position.z / cellSize)
        );

        moveCount = Mathf.Max(0, moveCount - 1);

        strategyManager?.UndoMove();

        GameEvents.RaiseMoveCountChanged(moveCount);

        Physics.SyncTransforms();

        RefreshSwitchState();
        CheckGoal();

        if (!isLevelCompleted)
        {
            GameEvents.RaisePlayerMoved(
                previousPosition,
                transform.position
            );
        }
    }

    private GridBlock FindBlockAt(Vector3 position)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            position,
            cellSize * 0.35f,
            hitBuffer
        );

        for (int i = 0; i < hitCount; i++)
        {
            GridBlock block =
                hitBuffer[i].GetComponentInParent<GridBlock>();

            if (block != null && block.enabled)
                return block;
        }

        return null;
    }

    private GridGoal FindGoalAt(Vector3 position)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            position,
            cellSize * 0.6f,
            hitBuffer
        );

        for (int i = 0; i < hitCount; i++)
        {
            GridGoal goal =
                hitBuffer[i].GetComponentInParent<GridGoal>();

            if (goal != null)
                return goal;
        }

        return null;
    }

    private void RefreshSwitchState()
    {
        GridSwitch[] switches =
            FindObjectsByType<GridSwitch>(FindObjectsSortMode.None);

        GridBlock[] blocks =
            FindObjectsByType<GridBlock>(FindObjectsSortMode.None);

        foreach (GridSwitch gridSwitch in switches)
        {
            if (gridSwitch == null)
                continue;

            bool occupied = IsSameGridCell(
                transform.position,
                gridSwitch.transform.position
            );

            if (!occupied)
            {
                foreach (GridBlock block in blocks)
                {
                    if (block == null || !block.enabled || !block.IsPushable)
                        continue;

                    if (IsSameGridCell(
                        block.transform.position,
                        gridSwitch.transform.position))
                    {
                        occupied = true;
                        break;
                    }
                }
            }

            gridSwitch.SetOccupied(occupied);
        }
    }

    private bool IsSameGridCell(Vector3 first, Vector3 second)
    {
        int firstX = Mathf.RoundToInt(first.x / cellSize);
        int firstZ = Mathf.RoundToInt(first.z / cellSize);

        int secondX = Mathf.RoundToInt(second.x / cellSize);
        int secondZ = Mathf.RoundToInt(second.z / cellSize);

        return firstX == secondX && firstZ == secondZ;
    }

    public void TeleportTo(Vector3 destination)
    {
        Vector3 previousPosition = transform.position;

        transform.position = new Vector3(
            destination.x,
            transform.position.y,
            destination.z
        );

        gridPosition = new Vector2Int(
            Mathf.RoundToInt(destination.x / cellSize),
            Mathf.RoundToInt(destination.z / cellSize)
        );

        Physics.SyncTransforms();

        RefreshSwitchState();
        CheckGoal();

        if (!isLevelCompleted)
        {
            GameEvents.RaisePlayerMoved(
                previousPosition,
                transform.position
            );
        }
    }

    private void CheckGoal()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            cellSize * 0.6f,
            hitBuffer
        );

        for (int i = 0; i < hitCount; i++)
        {
            GridGoal goal =
                hitBuffer[i].GetComponentInParent<GridGoal>();

            if (goal == null)
                continue;

            isLevelCompleted = true;

            goal.Complete();
            strategyManager?.CompleteLevel();
            return;
        }
    }

    private bool IsInsideGrid(Vector2Int position)
    {
        int minX = -gridWidth / 2;
        int maxX = gridWidth / 2;

        int minZ = -gridHeight / 2;
        int maxZ = gridHeight / 2;

        return position.x >= minX &&
               position.x <= maxX &&
               position.y >= minZ &&
               position.y <= maxZ;
    }

    private Vector3 GridToWorld(Vector2Int position)
    {
        return new Vector3(
            position.x * cellSize,
            transform.position.y,
            position.y * cellSize
        );
    }
}
