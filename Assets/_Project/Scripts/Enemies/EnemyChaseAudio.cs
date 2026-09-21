using UnityEngine;

public sealed class EnemyChaseAudio : MonoBehaviour
{
    [Header("Chase Music")]
    [SerializeField] private AudioClip chaseMusic;

    [SerializeField, Range(0f, 1f)]
    private float chaseMusicVolume = 1f;

    [Header("Background Music")]
    [SerializeField] private AudioSource backgroundAudioSource;

    [SerializeField, Range(0f, 1f)]
    private float backgroundVolume = 1f;

    [Header("Detection")]
    [SerializeField] private EnemyFOV enemyFOV;

    private AudioSource chaseAudioSource;
    private bool isChasing;

    private void Awake()
    {
        if (enemyFOV == null)
        {
            enemyFOV = GetComponent<EnemyFOV>();
        }

        chaseAudioSource = gameObject.AddComponent<AudioSource>();

        chaseAudioSource.playOnAwake = false;
        chaseAudioSource.loop = true;
        chaseAudioSource.volume = chaseMusicVolume;
        chaseAudioSource.spatialBlend = 0f;
        chaseAudioSource.clip = chaseMusic;
    }

    private void Update()
    {
        if (enemyFOV == null)
        {
            return;
        }

        bool playerDetected =
            enemyFOV.CurrentTarget != null;

        if (playerDetected && !isChasing)
        {
            StartChaseMusic();
        }
        else if (!playerDetected && isChasing)
        {
            StopChaseMusic();
        }
    }

    private void StartChaseMusic()
    {
        if (chaseMusic == null)
        {
            return;
        }

        isChasing = true;

    
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.volume = 0f;
        }

        if (!chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Play();
        }
    }

    private void StopChaseMusic()
    {
        isChasing = false;

        if (chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Stop();
        }

        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.volume = backgroundVolume;
        }
    }

    private void OnDestroy()
    {
        if (chaseAudioSource != null)
        {
            Destroy(chaseAudioSource);
        }
    }
}