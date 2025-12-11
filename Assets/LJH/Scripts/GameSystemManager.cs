using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystemManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "GravityScene";

    public void GoToNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("nextSceneNameError");
        }
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("GravityScene");
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        #else
            Application.Quit();
        #endif
    }
}
