using UnityEngine;

public sealed class HealthPickup : MonoBehaviour, IPickup
{
    [SerializeField] private float healAmount = 30f;

    public void Pickup(GameObject player)
    {
        if (player == null)
            return;

        TPShooter.Player.PlayerHealth health =
            player.GetComponentInParent<TPShooter.Player.PlayerHealth>();

        if (health == null)
        {
            Debug.LogWarning(
                $"{name}: PlayerHealth not found.",
                this);

            return;
        }

        health.Heal(healAmount);

        Destroy(gameObject);
    }
}