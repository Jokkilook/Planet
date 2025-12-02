using UnityEngine;

public class GravityField : MonoBehaviour
{
    [Header("기본 중력장 설정")]
    [SerializeField] private float gravityRadius = 10f;
    [SerializeField] private float gravityStrength = 50f;
    [SerializeField] private AnimationCurve gravityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("내부 강력 중력장 (빨아들이기)")]
    [SerializeField] private float innerRadius = 3.0f;      // 이 거리 안에 들어오면 작동
    [SerializeField] private float innerStrength = 100f;    // 훨씬 강력한 힘
    [SerializeField] private float innerDrag = 5.0f;        // 회전을 멈추게 할 강력한 마찰력
    private float defaultDrag = 0.1f;                       // 원래 발사체의 마찰력

    [Header("시각화")]
    [SerializeField] private bool showGravityField = true;
    [SerializeField] private Color fieldColor = new Color(0.5f, 0.5f, 1f, 0.3f);
    [SerializeField] private Color innerFieldColor = new Color(1f, 0.2f, 0.2f, 0.3f); // 내부 구역 색상
    
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
        ApplyGravity();
    }
    
    private void ApplyGravity()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, gravityRadius, affectedLayers);
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject == gameObject) continue;
            
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = transform.position - rb.position;
                float distance = direction.magnitude;
                
                // 거리가 너무 가까우면(0.1f 미만) 계산 제외 (발작 방지)
                if (distance <= 0.1f) continue;

                // [핵심 로직 변경]
                // 내부 반경(Inner Radius)에 들어왔는지 체크
                if (distance < innerRadius)
                {
                    // 1. 강력한 내부 중력 적용 (거리 상관없이 일정하게 강력한 힘 or 더 강한 힘)
                    // 방향 * 설정한 강력한 힘
                    rb.AddForce(direction.normalized * innerStrength);

                    // 2. 회전 관성을 죽이기 위해 드래그(Damping)를 높임
                    // 이렇게 하면 빙글빙글 도는 속도가 줄어들어 중심으로 직행함
                    rb.linearDamping = innerDrag; 
                }
                else // 일반 중력장 구역
                {
                    float normalizedDistance = distance / gravityRadius;
                    float gravityMultiplier = gravityCurve.Evaluate(normalizedDistance);
                    Vector3 gravityForce = direction.normalized * (gravityStrength * gravityMultiplier);
                    rb.AddForce(gravityForce);

                    // 원래 드래그로 복구 (나중에 다시 밖으로 나갈 수도 있으므로)
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
        
        // 외곽 중력장 (파란색)
        Gizmos.color = fieldColor;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
        
        // 내부 강력 중력장 (빨간색) - 새로 추가된 시각화
        Gizmos.color = innerFieldColor;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}