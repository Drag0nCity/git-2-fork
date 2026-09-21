using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameEndController : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";

    private bool gameEnded;

    public void FinishGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        Debug.Log("GAME WON!");

        SceneManager.LoadScene(menuSceneName);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }
}