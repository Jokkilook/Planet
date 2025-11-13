using UnityEngine;
using System.Collections.Generic;

public class PlanetShooter : MonoBehaviour
{
    public Transform bandCenter;
    public GameObject projectilePrefab;
    public float powerMultiplier = 10f;
    public int maxProjectiles = 10;

    Queue<Rigidbody> projectilePool = new Queue<Rigidbody>();

    void OnEnable() => PlanetShotEvents.OnRelease += HandleRelease;
    void OnDisable() => PlanetShotEvents.OnRelease -= HandleRelease;

    void Start()
    {
        // 간단한 Object Pool 생성
        for (int i = 0; i < maxProjectiles; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectilePool.Enqueue(obj.GetComponent<Rigidbody>());
        }
    }

    void HandleRelease(float tension, Vector3 centerPos, Vector3 pullPos)
    {
        if (projectilePool.Count == 0) return;

        Rigidbody rb = projectilePool.Dequeue();
        rb.gameObject.SetActive(true);

        rb.transform.position = centerPos;
        rb.transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 direction = (centerPos - pullPos).normalized;
        float force = tension * powerMultiplier;
        rb.AddForce(direction * force, ForceMode.Impulse);

        // 3초 후 비활성화 (풀 복귀)
        StartCoroutine(ReturnToPool(rb, 3f));
    }

    System.Collections.IEnumerator ReturnToPool(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.gameObject.SetActive(false);
        projectilePool.Enqueue(rb);
    }
}
