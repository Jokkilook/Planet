using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 씬 재시작을 위해 필요

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    // 게임 오버 패널 변수 제거됨

    [Header("Game State")]
    private int currentScore = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();

        // 시간 정상화 및 상태 초기화
        Time.timeScale = 1f;
        isGameOver = false;
    }

    /// <summary>
    /// 게임 오버 처리 (UI 없이 로직만 수행)
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Game Over!"); // 콘솔에 로그만 출력

        // 게임 정지 (물리 연산 및 시간 정지)
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 게임 재시작 (외부에서 호출 필요)
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        ResetScore();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 두 행성을 합쳐서 다음 단계 행성 생성
    /// </summary>
    public void MergePlanets(PlanetController planet1, PlanetController planet2)
    {
        if (isGameOver) return;

        GameObject nextPrefab = planet1.GetNextPlanetPrefab();

        if (nextPrefab == null)
        {
            Debug.LogWarning("다음 단계 프리팹이 설정되지 않았습니다!");
            return;
        }

        Vector3 mergePosition = (planet1.transform.position + planet2.transform.position) / 2f;

        Rigidbody rb1 = planet1.GetComponent<Rigidbody>();
        Rigidbody rb2 = planet2.GetComponent<Rigidbody>();
        Vector3 averageVelocity = Vector3.zero;

        if (rb1 != null && rb2 != null)
        {
            averageVelocity = (rb1.linearVelocity + rb2.linearVelocity) / 2f;
        }

        AddScore(planet1.GetMergeScore());

        Destroy(planet1.gameObject);
        Destroy(planet2.gameObject);

        GameObject newPlanet = Instantiate(nextPrefab, mergePosition, Quaternion.identity);

        Rigidbody newRb = newPlanet.GetComponent<Rigidbody>();
        if (newRb != null)
        {
            newRb.linearVelocity = averageVelocity;
        }
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
        {
            scoreText.text = "SCORE\n" + currentScore.ToString();
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }
}