using UnityEngine;

public class MoveComponent : MonoBehaviour
{

    public Transform centerA;   // 중심 A
    public Transform hmd;       // CenterEyeAnchor
    public float radius = 3f;   // 반지름 R
    public float rotateSpeed = 60f; // 궤도 이동 속도(도/초)

    float orbitAngle;     // 플레이어의 현재 궤도 각도(도 단위)
    float headingOffset;  // 플레이어가 유지해야 하는 시야 오프셋

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 초기 각도 설정
        Vector3 dir = (transform.position - centerA.position);
        dir.y = 0;
        orbitAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        if(input.magnitude > 0.1f){

            Debug.Log($"Input X : [ {input.x} ]");

            // 1) 공전 회전 업데이트
            orbitAngle += input.x * rotateSpeed * Time.deltaTime;

            // 2) 공전 위치 계산
            float rad = orbitAngle * Mathf.Deg2Rad;
            Vector3 newPos = centerA.position + new Vector3(
                Mathf.Cos(rad) * radius,
                transform.position.y,
                Mathf.Sin(rad) * radius
            );

            transform.position = newPos;

            // 3) 플레이어 방향 → 항상 A를 바라보게
            transform.LookAt(centerA.position);
        }
    }
}
