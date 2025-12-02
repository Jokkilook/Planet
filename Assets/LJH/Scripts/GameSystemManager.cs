using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystemManager : MonoBehaviour
{
    public void GoToNextScene()
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
