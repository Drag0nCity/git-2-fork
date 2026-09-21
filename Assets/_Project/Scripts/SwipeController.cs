using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private BoardRotator boardRotator;
    [SerializeField] private Camera mainCamera;

    [Header("Swipe Settings")]
    [SerializeField] private float minimumSwipeDistance = 50f;

    private Vector2 startPosition;
    private bool startedOnPlayer;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMouseInput();
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            startedOnPlayer = IsPointerOnPlayer(startPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipe = (Vector2)Input.mousePosition - startPosition;

            ProcessSwipe(swipe, startedOnPlayer);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startPosition = touch.position;
            startedOnPlayer = IsPointerOnPlayer(startPosition);
        }

        if (touch.phase == TouchPhase.Ended)
        {
            Vector2 swipe = touch.position - startPosition;

            ProcessSwipe(swipe, startedOnPlayer);
        }
    }

    private bool IsPointerOnPlayer(Vector2 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                -mainCamera.transform.position.z
            )
        );

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null)
            return false;

        return hit.transform == playerMovement.transform;
    }

    private void ProcessSwipe(Vector2 swipe, bool onPlayer)
    {
        if (swipe.magnitude < minimumSwipeDistance)
            return;

        if (onPlayer)
        {
            ProcessPlayerMovement(swipe);
        }
        else
        {
            ProcessBoardRotation(swipe);
        }
    }

    private void ProcessPlayerMovement(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0)
                playerMovement.Move(Vector2Int.right);
            else
                playerMovement.Move(Vector2Int.left);
        }
        else
        {
            if (swipe.y > 0)
                playerMovement.Move(Vector2Int.up);
            else
                playerMovement.Move(Vector2Int.down);
        }
    }

    private void ProcessBoardRotation(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0)
                boardRotator.RotateRight();
            else
                boardRotator.RotateLeft();
        }
        else
        {
            if (swipe.y > 0)
                boardRotator.RotateRight();
            else
                boardRotator.RotateLeft();
        }
    }
}