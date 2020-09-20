using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour {

    [SerializeField] private Canvas pauseMenuGUI;

    private static PauseMenuManager instance;
    private GameManager.GameState previousGameState;

    void Awake()
    {
        // Enforce singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static PauseMenuManager GetInstance()
    {
        return instance;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenuGUI.gameObject.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenuGUI.gameObject.SetActive(true);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
