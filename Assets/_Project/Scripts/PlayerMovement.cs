using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardGrid boardGrid;
    [SerializeField] private PathVisualizer pathVisualizer;
    [SerializeField] private BoardRotator boardRotator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2Int currentCell;
    private Vector3 targetPosition;
    private bool isMoving;

    private void OnEnable()
    {
        if (boardRotator != null)
        {
            boardRotator.RotationFinished += OnBoardRotationFinished;
        }
    }

    private void OnDisable()
    {
        if (boardRotator != null)
        {
            boardRotator.RotationFinished -= OnBoardRotationFinished;
        }
    }

    private void Start()
    {
        currentCell = boardGrid.PlayerStartCell;

        targetPosition = boardGrid.CellToLocal(currentCell);

        transform.localPosition = targetPosition;

        UpdatePathVisual();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(
                transform.localPosition,
                targetPosition) < 0.01f)
        {
            transform.localPosition = targetPosition;
            isMoving = false;

            UpdatePathVisual();
        }
    }

    public void Move(Vector2Int screenDirection)
    {
        if (isMoving)
            return;

        if (boardRotator != null &&
            boardRotator.IsRotating)
        {
            return;
        }
        Vector2Int boardDirection =
            ConvertScreenDirectionToBoardDirection(
                screenDirection
            );

        if (!boardGrid.CanMove(
                currentCell,
                boardDirection))
        {
            Debug.Log("Этот путь сейчас недоступен!");
            return;
        }

        Vector2Int nextCell =
            currentCell + boardDirection;

        if (!boardGrid.IsWalkable(nextCell))
        {
            Debug.Log("Нельзя двигаться туда!");
            return;
        }

        currentCell = nextCell;

        targetPosition =
            boardGrid.CellToLocal(currentCell);

        isMoving = true;
    }

    private Vector2Int ConvertScreenDirectionToBoardDirection(
        Vector2Int screenDirection)
    {
        if (boardRotator == null)
            return screenDirection;

        int rotation =
            boardRotator.CurrentRotation;

        rotation =
            ((rotation % 360) + 360) % 360;

        switch (rotation)
        {
            case 0:
                return screenDirection;

            case 90:
                return new Vector2Int(
                    screenDirection.y,
                    -screenDirection.x
                );

            case 180:
                return new Vector2Int(
                    -screenDirection.x,
                    -screenDirection.y
                );

            case 270:
                return new Vector2Int(
                    -screenDirection.y,
                    screenDirection.x
                );

            default:
                return screenDirection;
        }
    }

    private void OnBoardRotationFinished()
    {
        UpdatePathVisual();
    }

    private void UpdatePathVisual()
    {
        if (pathVisualizer == null)
            return;

        pathVisualizer.UpdateVisuals(
            currentCell,
            boardGrid
        );
    }
}