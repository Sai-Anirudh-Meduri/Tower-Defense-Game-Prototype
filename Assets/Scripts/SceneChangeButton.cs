using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public string targetScene;

    public void ChangeScene()
    {
        Time.timeScale = 1f;  // Unpause before loading scene
        SceneManager.LoadScene(targetScene);
    }
}
