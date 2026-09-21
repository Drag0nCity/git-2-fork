using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;

    private bool levelCompleted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted)
            return;

        if (!other.CompareTag("Player"))
            return;

        levelCompleted = true;

        Debug.Log("УРОВЕНЬ ПРОЙДЕН!");

        winPanel.SetActive(true);
    }
}