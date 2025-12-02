using UnityEngine;

public class GravityCore : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // ProjectileLauncher 스크립트에서 발사체 태그를 "Projectile"로 설정하고 있으므로 이를 확인
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                // 1. 속도와 회전력을 즉시 0으로 만듦
                // Unity 6 (2023.3+) 버전의 경우 linearVelocity 사용 (사용자 코드 기반)
                rb.linearVelocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero;

                // 2. 물리 연산을 끄기 (IsKinematic = true)
                // 이렇게 하면 GravityField가 가하는 AddForce도 무시하게 되어 굴러가지 않고 고정됨
                rb.isKinematic = true;

                // (선택 사항) 충돌한 물체를 이 구체의 자식으로 만들면,
                // 만약 이 구체가 움직일 때 같이 붙어서 움직이게 됩니다.
                rb.transform.SetParent(this.transform);
            }
        }
    }
}