using System;
using UnityEngine;

namespace TPShooter.Player
{
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;

        public event Action<float, float> HealthChanged;
        public event Action Died;

        private bool isDead;

        private void Awake()
        {
            maxHealth = Mathf.Max(1f, maxHealth);
            CurrentHealth = maxHealth;
            isDead = false;
        }

        private void Start()
        {
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"PLAYER DAMAGE: {damage}", this);

            if (isDead || damage <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(
                0f,
                CurrentHealth - damage);

            HealthChanged?.Invoke(
                CurrentHealth,
                maxHealth);

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (isDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Min(
                maxHealth,
                CurrentHealth + amount);

            HealthChanged?.Invoke(
                CurrentHealth,
                maxHealth);
        }

        private void Die()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            CurrentHealth = 0f;

            HealthChanged?.Invoke(
                CurrentHealth,
                maxHealth);

            Died?.Invoke();
        }

        public void ResetHealth()
        {
            isDead = false;
            CurrentHealth = maxHealth;

            HealthChanged?.Invoke(
                CurrentHealth,
                maxHealth);
        }
    }
}