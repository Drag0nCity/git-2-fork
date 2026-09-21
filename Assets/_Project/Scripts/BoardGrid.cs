using UnityEngine;

public class BoardGrid : MonoBehaviour
{
    public enum LevelLayout
    {
        Level1,
        Level2,
        Level3
    }

    [Header("Level")]
    [SerializeField] private LevelLayout levelLayout = LevelLayout.Level1;

    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridOrigin = new Vector2(-1f, -1f);

    [Header("Player Start")]
    [SerializeField] private Vector2Int playerStartCell = new Vector2Int(0, 0);

    public float CellSize => cellSize;
    public Vector2 GridOrigin => gridOrigin;
    public Vector2Int PlayerStartCell => playerStartCell;

    public Vector3 CellToLocal(Vector2Int cell)
    {
        return new Vector3(
            gridOrigin.x + cell.x * cellSize,
            gridOrigin.y + cell.y * cellSize,
            -0.2f
        );
    }

    public bool IsInsideGrid(Vector2Int cell)
    {
        return cell.x >= 0 &&
               cell.x <= 2 &&
               cell.y >= 0 &&
               cell.y <= 2;
    }

    public bool IsWalkable(Vector2Int cell)
    {
        return IsInsideGrid(cell);
    }

    public bool CanMove(
        Vector2Int fromCell,
        Vector2Int direction)
    {
        Vector2Int targetCell =
            fromCell + direction;

        if (!IsInsideGrid(targetCell))
            return false;

        switch (levelLayout)
        {
            case LevelLayout.Level1:
                return CanMoveLevel1(
                    fromCell,
                    direction
                );

            case LevelLayout.Level2:
                return CanMoveLevel2(
                    fromCell,
                    direction
                );

            case LevelLayout.Level3:
                return CanMoveLevel3(
                    fromCell,
                    direction
                );
        }

        return false;
    }

    private bool CanMoveLevel1(
        Vector2Int fromCell,
        Vector2Int direction)
    {

        if (fromCell == new Vector2Int(0, 0))
        {
            return direction == Vector2Int.right;
        }

        if (fromCell == new Vector2Int(1, 0))
        {
            return direction == Vector2Int.left ||
                   direction == Vector2Int.right;
        }

        if (fromCell == new Vector2Int(2, 0))
        {
            return direction == Vector2Int.left ||
                   direction == Vector2Int.up;
        }

        if (fromCell == new Vector2Int(2, 1))
        {
            return direction == Vector2Int.down ||
                   direction == Vector2Int.up;
        }

        if (fromCell == new Vector2Int(2, 2))
        {
            return direction == Vector2Int.down;
        }

        return false;
    }

    private bool CanMoveLevel2(
        Vector2Int fromCell,
        Vector2Int direction)
    {

        if (fromCell == new Vector2Int(0, 0))
        {
            return direction == Vector2Int.up;
        }

        if (fromCell == new Vector2Int(0, 1))
        {
            return direction == Vector2Int.down ||
                   direction == Vector2Int.up;
        }

        if (fromCell == new Vector2Int(0, 2))
        {
            return direction == Vector2Int.down ||
                   direction == Vector2Int.right;
        }

        if (fromCell == new Vector2Int(1, 2))
        {
            return direction == Vector2Int.left ||
                   direction == Vector2Int.right;
        }

        if (fromCell == new Vector2Int(2, 2))
        {
            return direction == Vector2Int.left;
        }

        return false;
    }

    private bool CanMoveLevel3(
        Vector2Int fromCell,
        Vector2Int direction)
    {
       

        if (fromCell == new Vector2Int(0, 0))
        {
            return direction == Vector2Int.right;
        }

        if (fromCell == new Vector2Int(1, 0))
        {
            return direction == Vector2Int.left ||
                   direction == Vector2Int.up;
        }

        if (fromCell == new Vector2Int(1, 1))
        {
            return direction == Vector2Int.down ||
                   direction == Vector2Int.up;
        }

        if (fromCell == new Vector2Int(1, 2))
        {
            return direction == Vector2Int.down ||
                   direction == Vector2Int.left ||
                   direction == Vector2Int.right;
        }

        if (fromCell == new Vector2Int(2, 2))
        {
            return direction == Vector2Int.left;
        }

        return false;
    }
}