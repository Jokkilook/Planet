using UnityEngine;

public class GravityField : MonoBehaviour
{
    [Header("기본 중력장 설정")]
    [SerializeField] private float gravityRadius = 10f;
    [SerializeField] private float gravityStrength = 50f;
    [SerializeField] private AnimationCurve gravityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("내부 강력 중력장 (빨아들이기)")]
    [SerializeField] private float innerRadius = 3.0f;
    [SerializeField] private float innerStrength = 100f;
    [SerializeField] private float innerDrag = 5.0f;
    private float defaultDrag = 0.1f;

    [Header("게임 오버 조건 설정")]
    [Tooltip("조건 1: 이 반경 밖으로 나가면 무조건 게임 오버 (최대 경계)")]
    [SerializeField] private float maxBoundaryRadius = 15.0f;

    [Tooltip("조건 2: 이 반경 안으로 들어왔던 행성이 다시 나가면 게임 오버 (궤도 이탈)")]
    [SerializeField] private float orbitCommitRadius = 8.0f;

    [Header("시각화")]
    [SerializeField] private bool showGravityField = true;
    [SerializeField] private Color fieldColor = new Color(0.5f, 0.5f, 1f, 0.3f);
    [SerializeField] private Color innerFieldColor = new Color(1f, 0.2f, 0.2f, 0.3f);
    [SerializeField] private Color boundaryColor = new Color(1f, 0f, 0f, 0.5f); // 게임오버 라인 색상
    [SerializeField] private Color orbitCommitColor = new Color(1f, 1f, 0f, 0.5f); // 궤도 진입 라인 색상

    [Header("성능")]
    [SerializeField] private LayerMask affectedLayers = -1;

    private SphereCollider triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<SphereCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<SphereCollider>();
        }
        triggerCollider.isTrigger = true;
        triggerCollider.radius = gravityRadius;
    }

    void FixedUpdate()
    {
        ApplyGravityAndCheckGameOver();
    }

    private void ApplyGravityAndCheckGameOver()
    {
        // 중력장 범위 내의 물체들 탐색
        // 주의: maxBoundaryRadius가 gravityRadius보다 크다면, 탐색 반경을 maxBoundaryRadius로 맞춰야 감지가 가능함
        float searchRadius = Mathf.Max(gravityRadius, maxBoundaryRadius);
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, affectedLayers);

        foreach (Collider col in colliders)
        {
            if (col.gameObject == gameObject) continue;

            PlanetController planet = col.GetComponent<PlanetController>();
            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 direction = transform.position - rb.position;
                float distance = direction.magnitude;

                // ---------------------------------------------------
                // [게임 오버 로직 체크]
                // ---------------------------------------------------
                if (planet != null) // 행성인 경우에만 체크
                {
                    // 조건 1: 그냥 최대 경계 밖으로 나감
                    if (distance > maxBoundaryRadius)
                    {
                        Debug.Log("게임 오버: 최대 경계 이탈!");
                        GameManager.Instance.GameOver(); // 게임매니저에 GameOver 함수가 있다고 가정
                        return;
                    }

                    // 조건 2: 궤도 진입 후 이탈 체크
                    // 2-1. 진입 체크: 궤도 라인 안으로 들어왔다면 플래그 설정
                    if (distance < orbitCommitRadius)
                    {
                        planet.HasEnteredOrbit = true;
                    }

                    // 2-2. 이탈 체크: 들어왔던 놈이 다시 나가면 죽음
                    // (약간의 오차 허용을 위해 0.5f 정도 여유를 주거나, 바로 체크해도 됨)
                    if (planet.HasEnteredOrbit && distance > orbitCommitRadius)
                    {
                        Debug.Log("게임 오버: 궤도 진입 후 이탈!");
                        GameManager.Instance.GameOver();
                        return;
                    }
                }
                // ---------------------------------------------------


                // [기존 중력 로직]
                // 중력은 gravityRadius 안에서만 작용해야 함
                if (distance > gravityRadius) continue;

                if (distance <= 0.1f) continue;

                if (distance < innerRadius)
                {
                    rb.AddForce(direction.normalized * innerStrength);
                    rb.linearDamping = innerDrag;
                }
                else
                {
                    float normalizedDistance = distance / gravityRadius;
                    float gravityMultiplier = gravityCurve.Evaluate(normalizedDistance);
                    Vector3 gravityForce = direction.normalized * (gravityStrength * gravityMultiplier);
                    rb.AddForce(gravityForce);
                    rb.linearDamping = defaultDrag;
                }
            }
        }
    }

    // UI 등에서 제어할 때 사용
    public void SetGravityStrength(float strength) => gravityStrength = strength;
    public void SetGravityRadius(float radius)
    {
        gravityRadius = radius;
        if (triggerCollider != null) triggerCollider.radius = radius;
    }

    public float GetGravityStrength() => gravityStrength;
    public float GetGravityRadius() => gravityRadius;

    void OnDrawGizmos()
    {
        if (!showGravityField) return;

        // 1. 기본 중력장 (파란색)
        Gizmos.color = fieldColor;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);

        // 2. 내부 강력 중력장 (빨간색)
        Gizmos.color = innerFieldColor;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        // 3. [게임오버] 최대 경계선 (진한 빨강 실선)
        Gizmos.color = boundaryColor;
        Gizmos.DrawWireSphere(transform.position, maxBoundaryRadius);

        // 4. [게임오버] 궤도 진입/이탈 라인 (노란색 실선)
        Gizmos.color = orbitCommitColor;
        Gizmos.DrawWireSphere(transform.position, orbitCommitRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}