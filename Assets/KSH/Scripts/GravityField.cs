using UnityEngine;
using System.Collections.Generic;

public class GravityField : MonoBehaviour
{
    [Header("중력장 설정")]
    [SerializeField] private float gravityRadius = 10f;
    [SerializeField] private float gravityStrength = 50f;
    [SerializeField] private AnimationCurve gravityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("시각화")]
    [SerializeField] private bool showGravityField = true;
    [SerializeField] private Color fieldColor = new Color(0.5f, 0.5f, 1f, 0.3f);
    
    [Header("성능")]
    [SerializeField] private LayerMask affectedLayers = -1;
    
    private SphereCollider triggerCollider;
    
    void Start()
    {
        // 트리거 콜라이더 자동 설정
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
                
                if (distance > 0.1f && distance < gravityRadius)
                {
                    float normalizedDistance = distance / gravityRadius;
                    float gravityMultiplier = gravityCurve.Evaluate(normalizedDistance);
                    Vector3 gravityForce = direction.normalized * (gravityStrength * gravityMultiplier);
                    rb.AddForce(gravityForce);
                }
            }
        }
    }
    
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
        
        Gizmos.color = fieldColor;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
        
        Gizmos.color = new Color(fieldColor.r, fieldColor.g, fieldColor.b, fieldColor.a * 0.3f);
        Gizmos.DrawWireSphere(transform.position, gravityRadius * 0.66f);
        Gizmos.DrawWireSphere(transform.position, gravityRadius * 0.33f);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}