using UnityEngine;

public class PlanetController : MonoBehaviour
{
[Header("Planet Settings")]
    [SerializeField] private int planetLevel = 0; // 0~9 (9 = 태양)
    [SerializeField] private int mergeScore = 10; // 합칠 때 얻는 점수
    [SerializeField] private GameObject nextPlanetPrefab; // 다음 단계 행성 프리팹

    private bool isMerging = false; // 합치는 중 여부 (안전장치)

    private void OnCollisionEnter(Collision collision)
    {
        // 이미 합치는 중이면 무시
        if (isMerging) return;

        // 충돌한 오브젝트가 행성인지 확인
        PlanetController otherPlanet = collision.gameObject.GetComponent<PlanetController>();
        if (otherPlanet == null) return;

        // 상대 행성도 합치는 중이면 무시
        if (otherPlanet.isMerging) return;

        // InstanceID 비교: ID가 작은 쪽만 처리 (중복 방지)
        if (GetInstanceID() < otherPlanet.GetInstanceID()) return;

        // 같은 레벨인지 확인
        if (planetLevel != otherPlanet.planetLevel) return;

        // 합치기 시작
        isMerging = true;
        otherPlanet.isMerging = true;

        // 태양(레벨 9)인 경우
        if (planetLevel == 9)
        {
            // 점수만 추가하고 두 태양 제거
            GameManager.Instance.AddScore(mergeScore);
            Destroy(gameObject);
            Destroy(otherPlanet.gameObject);
        }
        else
        {
            // 일반 행성 합치기
            GameManager.Instance.MergePlanets(this, otherPlanet);
        }
    }

    // Getter 메서드들
    public int GetPlanetLevel()
    {
        return planetLevel;
    }

    public int GetMergeScore()
    {
        return mergeScore;
    }

    public GameObject GetNextPlanetPrefab()
    {
        return nextPlanetPrefab;
    }
}
