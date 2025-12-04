using UnityEngine;

public class LauncherFollower : MonoBehaviour
{
    public Transform player;   // 플레이어(HMD Camera 또는 XR Origin Transform)
    public Transform targetA;  // A 오브젝트 (행성 등)
    public float distanceFromA = 1.0f;  // A로부터 떨어질 거리
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;

    void LateUpdate()
    {
        if (!player || !targetA) return;

        // A → 플레이어 방향
        Vector3 dir = (player.position - targetA.position).normalized;

        // 로컬 축 구성 (dir을 forward처럼 사용)
        Vector3 forward = dir;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 up = Vector3.up;

        // offsetPosition을 dir-space 기준으로 변환
        Vector3 offset =
              right * offsetPosition.x
            + up    * offsetPosition.y
            + forward * offsetPosition.z;

        // 최종 위치: A에서 dir로 distance 이동 + offset 적용
        transform.position = targetA.position + forward * distanceFromA + offset;

        // A를 바라보기
        transform.LookAt(targetA.position);

        // 회전 보정
        transform.Rotate(offsetRotation, Space.Self);
    }
}
