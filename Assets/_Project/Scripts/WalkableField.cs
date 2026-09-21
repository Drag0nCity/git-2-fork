using UnityEngine;

public class WalkableField : MonoBehaviour
{
    [Header("Field")]
    [SerializeField] private Vector2Int startCell;
    [SerializeField] private Vector2Int endCell;

    public Vector2Int StartCell => startCell;
    public Vector2Int EndCell => endCell;

    public bool ContainsCell(Vector2Int cell)
    {
        return cell == startCell || cell == endCell;
    }

    public bool Connects(Vector2Int from, Vector2Int to)
    {
        return
            (from == startCell && to == endCell) ||
            (from == endCell && to == startCell);
    }
}