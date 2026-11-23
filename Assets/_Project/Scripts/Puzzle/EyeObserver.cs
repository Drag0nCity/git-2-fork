using System.Collections;
using UnityEngine;
using TMPro;

public class EyeObserver : MonoBehaviour
{
    [Header("Eye")]
    [SerializeField] private SpriteRenderer eyeSprite;

    [Header("Vision Zone")]
    [SerializeField] private EyeVisionZone visionZone;

    [Header("Warning Text")]
    [SerializeField] private TMP_Text warningText;

    [Header("Timings")]
    [SerializeField, Min(0f)] private float closedDuration = 7f;
    [SerializeField, Min(0f)] private float warningDuration = 3f;
    [SerializeField, Min(0f)] private float openDuration = 4f;

    [Header("Text Flash")]
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.1f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
    [SerializeField, Min(0.1f)] private float pulseSpeed = 3f;

    private bool isOpen;
    private bool hasFailed;

    private void Awake()
    {
        if (eyeSprite == null)
            eyeSprite = GetComponent<SpriteRenderer>();

        if (eyeSprite == null)
            Debug.LogError("EyeObserver: SpriteRenderer is not assigned!", this);

        if (visionZone == null)
            Debug.LogError("EyeObserver: Vision Zone is not assigned!", this);

        if (warningText == null)
            Debug.LogError("EyeObserver: Warning Text is not assigned!", this);

        SetClosed();
        HideWarning();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerMoved += HandlePlayerMoved;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerMoved -= HandlePlayerMoved;
    }

    private void Start()
    {
        StartCoroutine(EyeCycle());
    }

    private IEnumerator EyeCycle()
    {
        while (!hasFailed)
        {
            SetClosed();

            yield return new WaitForSeconds(closedDuration);

            if (hasFailed)
                yield break;

            yield return StartCoroutine(ShowWarning());

            if (hasFailed)
                yield break;

            HideWarning();
            SetOpen();

            yield return new WaitForSeconds(openDuration);

            SetClosed();
        }
    }

    private IEnumerator ShowWarning()
    {
        if (warningText == null)
            yield break;

        warningText.gameObject.SetActive(true);

        Color color = warningText.color;
        float elapsed = 0f;

        while (elapsed < warningDuration && !hasFailed)
        {
            elapsed += Time.deltaTime;

            float pulse = Mathf.PingPong(elapsed * pulseSpeed, 1f);
            pulse = Mathf.SmoothStep(0f, 1f, pulse);

            color.a = Mathf.Lerp(minAlpha, maxAlpha, pulse);
            warningText.color = color;

            yield return null;
        }
    }

    private void SetClosed()
    {
        isOpen = false;

        if (eyeSprite != null)
            eyeSprite.color = Color.black;

        if (visionZone != null)
            visionZone.SetVisible(false);
    }

    private void SetOpen()
    {
        isOpen = true;

        if (eyeSprite != null)
            eyeSprite.color = Color.white;

        if (visionZone != null)
            visionZone.SetVisible(true);
    }

    private void HideWarning()
    {
        if (warningText == null)
            return;

        Color color = warningText.color;
        color.a = 0f;
        warningText.color = color;

        warningText.gameObject.SetActive(false);
    }

    private void HandlePlayerMoved(Vector3 previousPosition, Vector3 currentPosition)
    {
        if (!isOpen || hasFailed || visionZone == null)
            return;

        bool wasVisible = visionZone.ContainsPosition(previousPosition);
        bool isVisible = visionZone.ContainsPosition(currentPosition);

        if (wasVisible || isVisible)
        {
            hasFailed = true;

            Debug.Log("EyeObserver: Player was caught!");

            GameEvents.RaiseLevelFailed();
        }
    }
}
