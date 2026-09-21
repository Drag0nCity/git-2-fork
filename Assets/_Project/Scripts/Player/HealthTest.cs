using UnityEngine;
using UnityEngine.InputSystem;

namespace TPShooter.Player
{
    public sealed class HealthTest : MonoBehaviour
    {
        [SerializeField] private PlayerHealth health;

        private void Update()
        {
            if (Keyboard.current != null &&
                Keyboard.current.hKey.wasPressedThisFrame)
            {
                health.TakeDamage(10f);

                Debug.Log(
                    $"Player HP: {health.CurrentHealth}/{health.MaxHealth}"
                );
            }
        }
    }
}