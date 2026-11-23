using UnityEngine;
using TMPro;

public class MoveCounterObserver : MonoBehaviour
{
    [SerializeField] private TMP_Text moveCounterText;

    private void OnEnable()
    {
        GameEvents.OnMoveCountChanged += UpdateMoveCounter;
    }

    private void OnDisable()
    {
        GameEvents.OnMoveCountChanged -= UpdateMoveCounter;
    }

    private void UpdateMoveCounter(int moveCount)
    {
        if (moveCounterText != null)
        {
            moveCounterText.text = "Moves: " + moveCount;
        }
    }
}