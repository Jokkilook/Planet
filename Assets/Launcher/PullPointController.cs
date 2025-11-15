using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class PullPointController : MonoBehaviour
{
    [Header("Band Settings")]
    public Transform bandCenter;
    public Transform restPosition;
    public float maxStretch = 0.6f;
    public float minTension = 0.1f;
    public float returnSpeed = 8f;

    [Header("Trajectory Settings")]
    public int maxPoints = 50;               // 최대 점 개수
    public float pointSpacing = 0.02f;       // 점 간 거리 (월드 단위)
    public float pointScale = 0.02f;
    public float launchForceMultiplier = 10f;
    public GameObject dotPrefab;             // 작은 Sphere prefab
    private List<GameObject> dots = new List<GameObject>();

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isGrabbed = false;
    private Transform grabbingInteractor;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        grabbingInteractor = args.interactorObject.transform;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        grabbingInteractor = null;
        ClearDots();

        float tension = Vector3.Distance(bandCenter.position, transform.position);
        if (tension < minTension) return;

        PlanetShotEvents.InvokeOnRelease(tension, bandCenter.position, transform.position);
    }

    void Update()
    {
        if (isGrabbed && grabbingInteractor != null)
        {
            Vector3 dir = grabbingInteractor.position - bandCenter.position;
            float dist = Mathf.Min(dir.magnitude, maxStretch);
            transform.position = bandCenter.position + dir.normalized * dist;

            float tension = Vector3.Distance(bandCenter.position, transform.position);
            if (tension > minTension)
                ShowTrajectoryWithDots(dir.normalized * dist);
            else
                ClearDots();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, restPosition.position, Time.deltaTime * returnSpeed);
            ClearDots();
        }
    }

    void ShowTrajectoryWithDots(Vector3 pullVector)
    {
        ClearDots();

        float tension = pullVector.magnitude;
        if (tension < 0.01f) return;

        Vector3 launchDirection = (bandCenter.position - transform.position).normalized;
        Vector3 launchVelocity = launchDirection * tension * launchForceMultiplier;
        Vector3 startPosition = transform.position;

        float velocityMag = launchVelocity.magnitude;
        if (velocityMag < 0.01f) velocityMag = 0.01f; // 안전값

        // 점 간격 기준으로 필요한 점 개수 계산
        int visiblePoints = Mathf.Min(maxPoints, Mathf.CeilToInt(velocityMag * 5f / pointSpacing));

        for (int i = 0; i < visiblePoints; i++)
        {
            float t = i * pointSpacing / velocityMag; // velocity에 맞춘 시간 간격
            Vector3 point = startPosition + launchVelocity * t + 0.5f * Physics.gravity * t * t;

            GameObject dot = Instantiate(dotPrefab, point, Quaternion.identity);
            dot.transform.localScale = Vector3.one * pointScale;
            dots.Add(dot);
        }
    }

    void ClearDots()
    {
        foreach (var d in dots)
            Destroy(d);
        dots.Clear();
    }
}
