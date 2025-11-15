using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game State")]
    private int currentScore = 0;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    /// <summary>
    /// 두 행성을 합쳐서 다음 단계 행성 생성
    /// </summary>
    public void MergePlanets(PlanetController planet1, PlanetController planet2)
    {
        // 다음 단계 프리팹 가져오기
        GameObject nextPrefab = planet1.GetNextPlanetPrefab();
        
        if (nextPrefab == null)
        {
            Debug.LogWarning("다음 단계 프리팹이 설정되지 않았습니다!");
            return;
        }

        // 두 행성의 중간 위치 계산
        Vector3 mergePosition = (planet1.transform.position + planet2.transform.position) / 2f;

        // 두 행성의 평균 속도 계산 (합쳐진 행성에 자연스럽게 적용)
        Rigidbody rb1 = planet1.GetComponent<Rigidbody>();
        Rigidbody rb2 = planet2.GetComponent<Rigidbody>();
        Vector3 averageVelocity = Vector3.zero;
        
        if (rb1 != null && rb2 != null)
        {
            averageVelocity = (rb1.linearVelocity + rb2.linearVelocity) / 2f;
        }

        // 점수 추가
        AddScore(planet1.GetMergeScore());

        // 기존 행성 제거
        Destroy(planet1.gameObject);
        Destroy(planet2.gameObject);

        // 새 행성 생성
        GameObject newPlanet = Instantiate(nextPrefab, mergePosition, Quaternion.identity);

        // 새 행성에 속도 적용 (자연스러운 물리 효과)
        Rigidbody newRb = newPlanet.GetComponent<Rigidbody>();
        if (newRb != null)
        {
            newRb.linearVelocity = averageVelocity;
        }
    }

    /// <summary>
    /// 점수 추가
    /// </summary>
    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreUI();
    }

    /// <summary>
    /// 점수 UI 업데이트
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE\n" + currentScore;
        }
    }

    /// <summary>
    /// 현재 점수 반환
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }

    /// <summary>
    /// 점수 초기화 (게임 재시작 시 사용)
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }
}
