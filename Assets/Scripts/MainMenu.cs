using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        if (GameObject.Find("PauseMenuCanvas") == null)
        {
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        }
        if (GameObject.Find("GameOverCanvas") == null)
        {
            SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
