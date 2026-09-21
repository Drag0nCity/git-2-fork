using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanelController : MonoBehaviour
{
    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void NextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level_1")
        {
            SceneManager.LoadScene("Level_2");
        }
        else if (currentScene == "Level_2")
        {
            SceneManager.LoadScene("Level_3");
        }
        else if (currentScene == "Level_3")
        {
            SceneManager.LoadScene("Level_1");
        }
    }
}