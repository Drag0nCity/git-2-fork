using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    [Header("Path Objects")]
    [SerializeField] private GameObject[] horizontalPaths;
    [SerializeField] private GameObject[] verticalPaths;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color activeColor = Color.yellow;

    public void UpdateVisuals(
        Vector2Int currentCell,
        BoardGrid boardGrid)
    {
        SetAllPathsNormal();

        bool canLeft = boardGrid.CanMove(
            currentCell,
            Vector2Int.left
        );

        bool canRight = boardGrid.CanMove(
            currentCell,
            Vector2Int.right
        );

        bool canUp = boardGrid.CanMove(
            currentCell,
            Vector2Int.up
        );

        bool canDown = boardGrid.CanMove(
            currentCell,
            Vector2Int.down
        );

        if (canLeft || canRight)
        {
            int index = currentCell.y;

            if (index >= 0 &&
                index < horizontalPaths.Length)
            {
                SetPathColor(
                    horizontalPaths[index],
                    activeColor
                );
            }
        }

   
        if (canUp || canDown)
        {
            int index = currentCell.x;

            if (index >= 0 &&
                index < verticalPaths.Length)
            {
                SetPathColor(
                    verticalPaths[index],
                    activeColor
                );
            }
        }
    }

    public void SetAllPathsNormal()
    {
        foreach (GameObject path in horizontalPaths)
        {
            SetPathColor(
                path,
                normalColor
            );
        }

        foreach (GameObject path in verticalPaths)
        {
            SetPathColor(
                path,
                normalColor
            );
        }
    }

    private void SetPathColor(
        GameObject path,
        Color color)
    {
        if (path == null)
            return;

        SpriteRenderer renderer =
            path.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = color;
        }
    }
}