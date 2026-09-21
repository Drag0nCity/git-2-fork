using DG.Tweening;
using UnityEngine;

public sealed class DoorController : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform door;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 0.6f;

    [Header("Interaction")]
      private bool isOpen;
    private bool isMoving;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Awake()
    {
        if (door == null)
        {
            door = transform;
        }

        closedRotation = door.localRotation;

        openRotation =
            closedRotation *
            Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Interact()
    {
        if (isMoving)
        {
            return;
        }

        isOpen = !isOpen;
        isMoving = true;

        door.DOLocalRotateQuaternion(
                isOpen ? openRotation : closedRotation,
                duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                isMoving = false;
            });
    }

    public bool IsOpen => isOpen;
}