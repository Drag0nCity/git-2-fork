using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TPShooter.Player
{
    public sealed class PlayerRespawn : MonoBehaviour
    {
        [Header("Respawn")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float respawnDelay = 2f;

        [Header("Death UI")]
        [SerializeField] private TextMeshProUGUI deathText;

        [Header("Death Messages")]
        private readonly string[] deathMessages =
        {
            "Namdia (┬┬﹏┬┬)",
            "Have a bad dream?",
            "STOP IT! STOP IT! STOP IT! STOP IT! STOP IT! STOP IT! STOP IT! STOP IT!",
            "Are you still try?",
            "Are you still remember who you are?",
            "The moral of the story's pretty clear,I suppose.",
            "still have time to change your mind?",
            "What's a pity...",
            "The opposite of everything I wanted to be."
        };

        private PlayerHealth playerHealth;
        private CharacterController characterController;
        private PlayerController playerController;

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
            characterController = GetComponent<CharacterController>();
            playerController = GetComponent<PlayerController>();

            if (deathText != null)
            {
                deathText.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.Died += OnPlayerDied;
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= OnPlayerDied;
            }
        }

        private void OnPlayerDied()
        {
            if (deathText != null)
            {
                // Выбираем случайную надпись
                int randomIndex = UnityEngine.Random.Range(
                    0,
                    deathMessages.Length
                );

                deathText.text = deathMessages[randomIndex];
                deathText.gameObject.SetActive(true);
            }

            if (playerController != null)
            {
                playerController.enabled = false;
            }

            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            if (spawnPoint != null)
            {
                if (characterController != null)
                {
                    characterController.enabled = false;
                }

                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;

                if (characterController != null)
                {
                    characterController.enabled = true;
                }
            }

            playerHealth.ResetHealth();

            if (playerController != null)
            {
                playerController.enabled = true;
            }

            if (deathText != null)
            {
                deathText.gameObject.SetActive(false);
            }
        }
    }
}