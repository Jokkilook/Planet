using UnityEngine;
using System.Collections.Generic;

public class PlanetShooter : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip shootSound;
    public float shootVolume = 1.0f;

    private AudioSource audioSource;

    public Transform bandCenter;
    public List<GameObject> projectilePrefabList;
    public float powerMultiplier = 10f;
    public int maxProjectiles = 10;

    Queue<GameObject> projectilePool = new Queue<GameObject>();

    public Transform previewAnchor;
    public float previewRotateSpeed = 30f;
    public float previewScale = 0.1f;
    public float tiltAngle = 23.5f;
    private GameObject previewObject;

    void OnEnable() => PlanetShotEvents.OnRelease += HandleRelease;
    void OnDisable() => PlanetShotEvents.OnRelease -= HandleRelease;

    void Start()
    {
        // 간단한 Object Pool 생성
        for (int i = 0; i < maxProjectiles; i++)
        {
            int index = Random.Range(0, projectilePrefabList.Count);
            GameObject prefab = projectilePrefabList[index];

            projectilePool.Enqueue(prefab); 
        }

        // ★ AudioSource 자동 생성
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;  // VR/3D 공간에서 들리게
        audioSource.volume = shootVolume;

        UpdatePreview();
    }

    void Update()
    {
        if (previewObject != null)
        {
            // 기울어진 자전축 계산
            Quaternion tilt = Quaternion.Euler(tiltAngle, 0, 0);
            Vector3 tiltedAxis = tilt * Vector3.up;   // 기울어진 Y축

            // 해당 축을 기준으로 자전
            previewObject.transform.Rotate(tiltedAxis, previewRotateSpeed * Time.deltaTime, Space.World);        
        }
    }

    void UpdatePreview()
    {
        if (projectilePool.Count == 0 || previewAnchor == null)
            return;

        GameObject prefab = projectilePool.Peek();

        if (previewObject != null)
            Destroy(previewObject);

        previewObject = Instantiate(prefab, previewAnchor);
        previewObject.transform.localPosition = Vector3.zero;
        previewObject.transform.localRotation = Quaternion.identity;
        previewObject.transform.localScale = Vector3.one * previewScale;

        DestroyImmediate(previewObject.GetComponent<Rigidbody>());
        DestroyImmediate(previewObject.GetComponent<Collider>());
    }

    void HandleRelease(float tension, Vector3 centerPos, Vector3 pullPos)
    {
        if (projectilePool.Count == 0) return;

        GameObject prefab = projectilePool.Dequeue();

        GameObject obj = Instantiate(prefab);
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        obj.SetActive(true);

        rb.transform.position = centerPos;
        rb.transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 dir = (centerPos - pullPos).normalized;
        rb.AddForce(dir * (tension * powerMultiplier), ForceMode.Impulse);

        // ★ 발사 소리 재생
        if (shootSound != null)
            audioSource.PlayOneShot(shootSound, shootVolume);

        // 새 프리팹을 다시 큐에 넣기
        int index = Random.Range(0, projectilePrefabList.Count);
        projectilePool.Enqueue(projectilePrefabList[index]);

        UpdatePreview();
    }

    public Vector3[] CalculateTrajectory(Vector3 startPos, Vector3 startVelocity, int steps = 30, float timeStep = 0.05f)
    {
        Vector3[] points = new Vector3[steps];

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            Vector3 pos = startPos + startVelocity * t + 0.5f * Physics.gravity * (t * t);

            points[i] = pos;
        }

        return points;
    }
}
