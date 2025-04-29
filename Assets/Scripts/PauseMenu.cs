using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;


        if (currentScene.IndexOf("Level") != 0 || currentScene == "LevelSelect")
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
            // if (GameOverManager.instance.isGamePaused)
            //     ResumeGame();
            // else
            //     PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        // GameOverManager.instance.isGamePaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        // GameOverManager.instance.isGamePaused = false;
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        // Clear the playersInGoal Hash Set when replay the game
        GameWin.playersInGoal.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        GameWin.playersInGoal.Clear();
        SceneManager.LoadScene("MainMenu");

    }
}
