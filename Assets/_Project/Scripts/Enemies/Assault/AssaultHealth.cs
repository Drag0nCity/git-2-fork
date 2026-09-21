using UnityEngine;

public sealed class AssaultHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    private bool _dead;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_dead || damage <= 0f)
        {
            return;
        }

        CurrentHealth =
            Mathf.Max(
                0f,
                CurrentHealth - damage);

        Debug.Log(
            $"ASSASSIN DAMAGE: {damage} | HP: {CurrentHealth}",
            this);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_dead)
        {
            return;
        }

        _dead = true;

        Debug.Log(
            "ASSASSIN DIED",
            this);

        AssaultController controller =
            GetComponent<AssaultController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        Destroy(
            gameObject,
            1.5f);
    }
}