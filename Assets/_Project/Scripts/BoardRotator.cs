using System;
using System.Collections;
using UnityEngine;

public class BoardRotator : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationDuration = 0.3f;

    private bool isRotating;
    private int currentRotation;

    public int CurrentRotation => currentRotation;
    public bool IsRotating => isRotating;

    public event Action RotationFinished;

    public void RotateRight()
    {
        if (isRotating)
            return;

        StartCoroutine(RotateBoard(90));
    }

    public void RotateLeft()
    {
        if (isRotating)
            return;

        StartCoroutine(RotateBoard(-90));
    }

    private IEnumerator RotateBoard(int angle)
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;

        currentRotation += angle;

        currentRotation = ((currentRotation % 360) + 360) % 360;

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, currentRotation);

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / rotationDuration
            );

            transform.rotation = Quaternion.Lerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        transform.rotation = targetRotation;

        isRotating = false;

        RotationFinished?.Invoke();
    }
}