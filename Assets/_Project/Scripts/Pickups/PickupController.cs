using UnityEngine;
using Zenject;

public sealed class PickupController : MonoBehaviour
{
    [SerializeField] private float pickupRadius = 5f;
    [SerializeField] private LayerMask pickupMask;

    private IInputService _inputService;

    private readonly Collider[] _buffer = new Collider[16];

    [Inject]
    private void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    private void Update()
    {
        if (_inputService == null)
            return;

        if (!_inputService.InteractPressed)
            return;

        Debug.Log("PICKUP: E PRESSED", this);

        TryPickup();
    }

    private void TryPickup()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            pickupRadius,
            _buffer,
            pickupMask
        );

        Debug.Log(
            $"PICKUP: Found {count} colliders",
            this
        );

        for (int i = 0; i < count; i++)
        {
            Collider collider = _buffer[i];

            if (collider == null)
                continue;

            Debug.Log(
                $"PICKUP: Found {collider.name}",
                collider
            );

            IPickup pickup =
                collider.GetComponentInParent<IPickup>();

            if (pickup == null)
            {
                Debug.LogWarning(
                    $"PICKUP: {collider.name} has no IPickup.",
                    collider
                );

                continue;
            }

            pickup.Pickup(gameObject);
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            pickupRadius
        );
    }
}