using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(GridBlock))]
public class GridDoor : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField, Min(1)] private int requiredSwitches = 1;

    private readonly HashSet<GridSwitch> registeredSwitches =
        new HashSet<GridSwitch>();

    private readonly HashSet<GridSwitch> occupiedSwitches =
        new HashSet<GridSwitch>();

    private GridBlock gridBlock;
    private BoxCollider doorCollider;
    private Renderer[] doorRenderers;

    private bool isOpen;
    private bool pendingClose;

    private Vector3 doorBoundsCenter;
    private Vector3 doorBoundsExtents;

    private void Awake()
    {
        gridBlock = GetComponent<GridBlock>();
        doorCollider = GetComponent<BoxCollider>();
        doorRenderers = GetComponentsInChildren<Renderer>();

        doorBoundsCenter = doorCollider.bounds.center;
        doorBoundsExtents = doorCollider.bounds.extents;

        ApplyDoorState(false);
    }

    private void Update()
    {
        if (pendingClose && !IsDoorwayOccupied())
        {
            pendingClose = false;
            ApplyDoorState(false);
        }
    }

    public void RegisterSwitch(GridSwitch gridSwitch)
    {
        if (gridSwitch == null)
            return;

        registeredSwitches.Add(gridSwitch);
    }

    public void UnregisterSwitch(GridSwitch gridSwitch)
    {
        if (gridSwitch == null)
            return;

        registeredSwitches.Remove(gridSwitch);
        occupiedSwitches.Remove(gridSwitch);

        RefreshDoorState();
    }

    public void SetSwitchOccupied(
        GridSwitch gridSwitch,
        bool occupied)
    {
        if (gridSwitch == null)
            return;

        if (!registeredSwitches.Contains(gridSwitch))
            return;

        if (occupied)
            occupiedSwitches.Add(gridSwitch);
        else
            occupiedSwitches.Remove(gridSwitch);

        RefreshDoorState();
    }

    private void RefreshDoorState()
    {
        bool shouldOpen =
            registeredSwitches.Count >= requiredSwitches &&
            occupiedSwitches.Count >= requiredSwitches;

        SetOpen(shouldOpen);
    }

    private void SetOpen(bool open)
    {
        if (open)
        {
            pendingClose = false;

            if (!isOpen)
                ApplyDoorState(true);

            return;
        }

        if (!isOpen)
            return;

        if (IsDoorwayOccupied())
        {
            pendingClose = true;
            return;
        }

        pendingClose = false;
        ApplyDoorState(false);
    }

    private bool IsDoorwayOccupied()
    {
        Collider[] hits = Physics.OverlapBox(
            doorBoundsCenter,
            doorBoundsExtents,
            Quaternion.identity,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            GridPlayerController player =
                hit.GetComponentInParent<GridPlayerController>();

            if (player != null)
                return true;

            GridBlock block =
                hit.GetComponentInParent<GridBlock>();

            if (block != null && block != gridBlock)
                return true;
        }

        return false;
    }

    private void ApplyDoorState(bool open)
    {
        isOpen = open;

        gridBlock.enabled = !isOpen;
        doorCollider.enabled = !isOpen;

        foreach (Renderer doorRenderer in doorRenderers)
        {
            doorRenderer.enabled = !isOpen;
        }

        Debug.Log(isOpen ? "Door opened" : "Door closed");
    }
}


