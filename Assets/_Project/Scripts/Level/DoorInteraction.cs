using UnityEngine;
using Zenject;

public sealed class DoorInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2.5f;
    [SerializeField] private LayerMask doorMask;

    private IInputService _inputService;

    private readonly Collider[] _buffer =
        new Collider[8];

    [Inject]
    private void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    private void Update()
    {
        if (_inputService == null)
        {
            return;
        }

        if (!_inputService.InteractPressed)
        {
            return;
        }

        TryOpenDoor();
    }

    private void TryOpenDoor()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRadius,
            _buffer,
            doorMask
        );

        for (int i = 0; i < count; i++)
        {
            if (_buffer[i] == null)
            {
                continue;
            }

            DoorController door =
                _buffer[i].GetComponentInParent<DoorController>();

            if (door == null)
            {
                continue;
            }

            PlayerKey playerKey =
                GetComponentInParent<PlayerKey>();

            if (playerKey == null || !playerKey.HasKey)
            {
                Debug.Log("Дверь заперта. Нужен ключ.");
                return;
            }

            door.Interact();
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            interactionRadius
        );
    }
}