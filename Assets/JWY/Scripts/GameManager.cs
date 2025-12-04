using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;

    private float remainingTime; 
    private float currentSessionTime = 0f; // 이번 판 플레이 시간
    private int currentScore = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        remainingTime = gameDuration;
        currentSessionTime = 0f; // 이번 판 시간 0초부터 시작
        currentScore = 0;
        isGameOver = false;
        Time.timeScale = 1f;

        UpdateScoreUI();
    }

    private void Update()
    {
        if (isGameOver) return;

        // 이번 판 플레이 시간 기록
        currentSessionTime += Time.deltaTime;

        // 타이머 로직
        if (remainingTime > 0)
        {
            remainingTime += Time.deltaTime;
            UpdateTimerUI();

            // if (remainingTime <= 0)
            // {
            //     remainingTime = 0;
            //     GameOver();
            // }
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Game Over!");

        // 1. 이번 판 점수 저장
        PlayerPrefs.SetInt("LastScore", currentScore);

        // 2. [핵심] 총 플레이 시간 누적 계산
        // 기존에 저장되어 있던 총 시간을 불러옴 (없으면 0)
        float previousTotalTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
        
        // 기존 총 시간 + 이번 판 시간
        float newTotalTime = previousTotalTime + currentSessionTime;
        
        // 합산된 시간을 다시 저장
        PlayerPrefs.SetFloat("TotalPlayTime", newTotalTime);
        PlayerPrefs.Save();

        SceneManager.LoadScene("ResultScene");
    }

    // --- 기존 함수들은 그대로 유지 ---
    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MergePlanets(PlanetController planet1, PlanetController planet2)
    {
        if (isGameOver) return;
        GameObject nextPrefab = planet1.GetNextPlanetPrefab();
        if (nextPrefab == null) return;
        Vector3 mergePosition = (planet1.transform.position + planet2.transform.position) / 2f;
        
        Rigidbody rb1 = planet1.GetComponent<Rigidbody>();
        Rigidbody rb2 = planet2.GetComponent<Rigidbody>();
        Vector3 averageVelocity = Vector3.zero;
        if (rb1 != null && rb2 != null) averageVelocity = (rb1.linearVelocity + rb2.linearVelocity) / 2f;

        AddScore(planet1.GetMergeScore());
        Destroy(planet1.gameObject);
        Destroy(planet2.gameObject);

        GameObject newPlanet = Instantiate(nextPrefab, mergePosition, Quaternion.identity);
        Rigidbody newRb = newPlanet.GetComponent<Rigidbody>();
        if (newRb != null) newRb.linearVelocity = averageVelocity;
    }

    public void AddScore(int score)
    {
        if (isGameOver) return;
        currentScore += score;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE : " + currentScore.ToString();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentSessionTime / 60F);
            int seconds = Mathf.FloorToInt(currentSessionTime % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            if (remainingTime <= 10f) timerText.color = Color.red;
            else timerText.color = Color.white;
        }
    }
}