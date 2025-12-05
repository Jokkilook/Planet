using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI totalPlayTimeText; // 이름 변경 (누적 시간)

    private void Start()
    {
        // 1. 점수 표시
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        if (finalScoreText != null)
        {
            finalScoreText.text = "FINAL SCORE\n" + lastScore.ToString();
        }

        // 2. [수정] 누적 총 플레이 시간 표시
        // "TotalPlayTime" 키값으로 불러옵니다.
        float totalTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
        
        if (totalPlayTimeText != null)
        {
            // 시간이 길어질 수 있으니 시:분:초로 표시하거나 분:초로 표시
            // 여기서는 깔끔하게 분:초로 표시합니다. (60분이 넘어가면 65:30 처럼 나옴)
            int minutes = Mathf.FloorToInt(totalTime / 60F);
            int seconds = Mathf.FloorToInt(totalTime % 60F);

            // Total Time : 125:40 (총 125분 40초 플레이함)
            totalPlayTimeText.text = string.Format("PLAY TIME\n" + "{0:00}:{1:00}", minutes, seconds);
        }
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene("GravityScene");
    }

    public void OnClickExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}