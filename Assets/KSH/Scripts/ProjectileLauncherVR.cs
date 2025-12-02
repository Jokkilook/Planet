using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.XR.Interaction.Toolkit; // XR Toolkit

public class ProjectileLauncherVR : MonoBehaviour
{
    [Header("VR 설정")]
    // XR Origin 아래에 있는 Right Controller 오브젝트를 여기에 연결하세요
    [SerializeField] private Transform rightHandController; 
    
    // 발사 버튼 (Input System의 Right Hand Trigger 버튼)
    [SerializeField] private InputActionProperty fireAction;

    [Header("발사 설정")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float launchForce = 20f; // VR은 드래그보다 보는 방향 발사가 직관적이라 고정 힘 사용 추천
    
    [Header("궤적 표시")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPoints = 30;
    
    private bool isAiming = false;

    void Start()
    {
        SetupTrajectoryLine();
    }
    
    void SetupTrajectoryLine()
    {
        if (trajectoryLine == null)
        {
            GameObject lineObj = new GameObject("TrajectoryLine");
            lineObj.transform.SetParent(transform);
            trajectoryLine = lineObj.AddComponent<LineRenderer>();
        }
        
        trajectoryLine.startWidth = 0.02f;
        trajectoryLine.endWidth = 0.01f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = Color.green;
        trajectoryLine.endColor = new Color(1f, 1f, 0f, 0.3f);
        trajectoryLine.enabled = false;
    }

    void Update()
    {
        // 트리거 버튼의 눌림 정도 (0.0 ~ 1.0)
        float triggerValue = fireAction.action.ReadValue<float>();

        // 트리거를 살짝이라도 누르면 조준 시작
        if (triggerValue > 0.1f)
        {
            isAiming = true;
            trajectoryLine.enabled = true;
            UpdateTrajectory();
        }
        // 조준 중이다가 트리거를 놓으면(0.1 이하) 발사
        else if (isAiming && triggerValue <= 0.1f)
        {
            Launch();
            isAiming = false;
            trajectoryLine.enabled = false;
        }
    }

    void Launch()
    {
        // 컨트롤러의 위치에서 컨트롤러가 향하는 방향(forward)으로 발사
        Vector3 spawnPos = rightHandController.position;
        Vector3 launchDir = rightHandController.forward;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        projectile.tag = "Projectile"; // 기존 로직 호환용 태그

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null) rb = projectile.AddComponent<Rigidbody>();
        
        rb.useGravity = false; 
        rb.linearVelocity = launchDir * launchForce; // Unity 6 (linearVelocity)

        Destroy(projectile, 30f);
    }

    void UpdateTrajectory()
    {
        Vector3 currentPos = rightHandController.position;
        Vector3 currentVel = rightHandController.forward * launchForce;

        Vector3[] points = new Vector3[trajectoryPoints];
        float timeStep = 0.1f;
        
        for (int i = 0; i < trajectoryPoints; i++)
        {
            points[i] = currentPos;
            currentPos += currentVel * timeStep;
            
            // 중력장 시뮬레이션 (기존 코드와 동일)
            GravityField[] fields = FindObjectsOfType<GravityField>();
            foreach (var field in fields)
            {
                Vector3 toField = field.transform.position - currentPos;
                float dist = toField.magnitude;
                if (dist < field.GetGravityRadius() && dist > 0.1f)
                {
                    float strength = field.GetGravityStrength() * 0.01f;
                    currentVel += toField.normalized * strength * timeStep;
                }
            }
        }
        
        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.SetPositions(points);
    }
}