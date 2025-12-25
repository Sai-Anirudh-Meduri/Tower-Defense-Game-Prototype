using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject gameplayUI;
    public SettingsButton settingsButton;
    public bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        settingsButton.ResetToDefault();
        
        gameplayUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        StartCoroutine(ReLockCursorWebGL());  // IMPORTANT
    }
    IEnumerator ReLockCursorWebGL()
    {
        yield return null;   // wait exactly 1 frame
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        gameplayUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
