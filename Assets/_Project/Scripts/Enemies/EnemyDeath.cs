using UnityEngine;

public sealed class EnemyDeath : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        enemyHealth.Died += OnDied;
    }

    private void OnDisable()
    {
        enemyHealth.Died -= OnDied;
    }

    private void OnDied()
    {
        gameObject.SetActive(false);
    }
}