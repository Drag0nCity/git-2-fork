using TMPro;
using UnityEngine;

namespace TPShooter.UI
{
    public sealed class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private Player.PlayerHealth playerHealth;
        [SerializeField] private TMP_Text healthText;

        private void Start()
        {
            playerHealth.HealthChanged += OnHealthChanged;

            OnHealthChanged(
                playerHealth.CurrentHealth,
                playerHealth.MaxHealth
            );
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
                playerHealth.HealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            healthText.text =
                $"HP: {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }
}