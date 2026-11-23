using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GridSwitch : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField] private GridDoor targetDoor;

    [Header("Timed Switch")]
    [SerializeField] private bool useTimer = false;
    [SerializeField, Min(1)] private int openForMoves = 3;

    private bool isOccupied;
    private bool wasPhysicallyOccupied;

    private int remainingMoves;
    private bool timerRunning;
    private bool skipActivationMove;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        if (targetDoor == null)
        {
            Debug.LogError(
                "GridSwitch: Target Door is not assigned!",
                this
            );
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerMoved += HandlePlayerMoved;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerMoved -= HandlePlayerMoved;
    }

    private void Start()
    {
        if (targetDoor != null)
        {
            targetDoor.RegisterSwitch(this);
            targetDoor.SetSwitchOccupied(this, isOccupied);
        }
    }

    public void SetOccupied(bool occupied)
    {
        if (!useTimer)
        {
            if (isOccupied == occupied)
                return;

            isOccupied = occupied;
            UpdateDoor();
            return;
        }

        bool justEntered = occupied && !wasPhysicallyOccupied;

        wasPhysicallyOccupied = occupied;

        if (justEntered && !timerRunning)
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        remainingMoves = Mathf.Max(1, openForMoves);
        timerRunning = true;

        SetActive(true);
               
        skipActivationMove = true;
        StartCoroutine(ResetSkipFlagNextFrame());

        Debug.Log(
            "Timed switch activated! Remaining moves: " + remainingMoves,
            this
        );
    }

    private IEnumerator ResetSkipFlagNextFrame()
    {
        yield return null;
        skipActivationMove = false;
    }

    private void HandlePlayerMoved(
        Vector3 previousPosition,
        Vector3 currentPosition)
    {
        if (!useTimer || !timerRunning)
            return;

        if (skipActivationMove)
        {
            skipActivationMove = false;
            return;
        }

        remainingMoves--;

        Debug.Log(
            "Timed switch remaining moves: " + remainingMoves,
            this
        );

        if (remainingMoves <= 0)
        {
            timerRunning = false;
            SetActive(false);

            Debug.Log("Timed switch expired!", this);
        }
    }

    private void SetActive(bool active)
    {
        if (isOccupied == active)
            return;

        isOccupied = active;
        UpdateDoor();
    }

    private void UpdateDoor()
    {
        if (targetDoor != null)
        {
            targetDoor.SetSwitchOccupied(this, isOccupied);
        }
    }

    private void OnDestroy()
    {
        if (targetDoor != null)
        {
            targetDoor.UnregisterSwitch(this);
        }
    }
}
