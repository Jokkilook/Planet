using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileLauncher : MonoBehaviour
{
    [Header("발사 설정")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float launchForceMultiplier = 10f;
    [SerializeField] private float maxLaunchForce = 50f;
    [SerializeField] private Transform launchPoint;
    
    [Header("궤적 표시")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPoints = 30;
    
    private Camera mainCamera;
    private Vector2 startMousePos;
    private Vector2 currentMousePos;
    private bool isDragging = false;
    
    // New Input System
    private Mouse mouse;
    private Keyboard keyboard;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (launchPoint == null) launchPoint = transform;
        
        // Input System 디바이스 참조
        mouse = Mouse.current;
        keyboard = Keyboard.current;
        
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
        
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.02f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = Color.green;
        trajectoryLine.endColor = new Color(1f, 1f, 0f, 0.3f);
        trajectoryLine.enabled = false;
    }
    
    void Update()
    {
        if (mouse == null || keyboard == null) return;
        
        // 마우스 위치 업데이트
        currentMousePos = mouse.position.ReadValue();
        
        // 마우스 왼쪽 버튼 클릭
        if (mouse.leftButton.wasPressedThisFrame)
        {
            StartDrag();
        }
        else if (mouse.leftButton.wasReleasedThisFrame && isDragging)
        {
            Launch();
        }
        
        // ESC로 취소
        if (keyboard.escapeKey.wasPressedThisFrame && isDragging)
        {
            CancelDrag();
        }
        
        // R키로 모든 발사체 제거
        if (keyboard.rKey.wasPressedThisFrame)
        {
            ClearAllProjectiles();
        }
        
        if (isDragging)
        {
            UpdateTrajectory();
        }
    }
    
    void StartDrag()
    {
        isDragging = true;
        startMousePos = currentMousePos;
        trajectoryLine.enabled = true;
    }
    
    void CancelDrag()
    {
        isDragging = false;
        trajectoryLine.enabled = false;
    }
    
    void Launch()
    {
        Vector3 launchVelocity = CalculateLaunchVelocity();
        
        GameObject projectile = CreateProjectile();
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = launchVelocity;
        
        Destroy(projectile, 30f);
        CancelDrag();
    }
    
    GameObject CreateProjectile()
    {
        GameObject projectile;
        
        if (projectilePrefab != null)
        {
            projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        }
        else
        {
            projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.transform.position = launchPoint.position;
            projectile.transform.localScale = Vector3.one * 0.3f;
            
            Renderer renderer = projectile.GetComponent<Renderer>();
            renderer.material.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
        }
        
        projectile.tag = "Projectile";
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null) rb = projectile.AddComponent<Rigidbody>();
        
        rb.useGravity = false;
        rb.linearDamping = 0.1f;
        
        return projectile;
    }
    
    Vector3 CalculateLaunchVelocity()
    {
        Vector2 dragVector = currentMousePos - startMousePos;
        
        float force = Mathf.Min(dragVector.magnitude / Screen.height * launchForceMultiplier, maxLaunchForce);
        
        // 2D 드래그를 3D 방향으로 변환
        Vector3 direction = new Vector3(dragVector.x, dragVector.y, dragVector.y).normalized;
        direction = mainCamera.transform.TransformDirection(direction);
        
        return direction * force;
    }
    
    void UpdateTrajectory()
    {
        Vector3 velocity = CalculateLaunchVelocity();
        Vector3[] points = new Vector3[trajectoryPoints];
        
        float timeStep = 0.1f;
        Vector3 currentPos = launchPoint.position;
        Vector3 currentVel = velocity;
        
        for (int i = 0; i < trajectoryPoints; i++)
        {
            points[i] = currentPos;
            currentPos += currentVel * timeStep;
            
            // 중력장 영향 시뮬레이션
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
    
    void ClearAllProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (var p in projectiles) Destroy(p);
        Debug.Log($"{projectiles.Length}개 발사체 제거됨");
    }
    
    void OnGUI()
    {
        if (!isDragging) return;
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        
        float force = CalculateLaunchVelocity().magnitude;
        
        GUI.Label(new Rect(currentMousePos.x + 20, Screen.height - currentMousePos.y, 200, 30), 
            $"Force: {force:F1}", style);
    }
}