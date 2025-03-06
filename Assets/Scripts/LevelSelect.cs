using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene("Level" + levelName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
