using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;

public class PullPointController : MonoBehaviour
{
    [Header("Band Settings")]
    public Transform bandCenter;
    public Transform restPosition;
    public float maxStretch = 0.6f;
    public float minTension = 0.1f;
    public float returnSpeed = 8f;

    [Header("Trajectory Settings")]
    public int maxPoints = 50;
    public float pointSpacing = 0.02f;
    public float pointScale = 0.02f;
    public float launchForceMultiplier = 10f;
    public GameObject dotPrefab;

    private List<GameObject> dots = new List<GameObject>();

    private bool isGrabbed = false;
    private Transform grabbingInteractor;

    // ============================
    //   EVENT: GRAB SELECTED
    // ============================
    public void HandleGrab()
    {
        Debug.Log("[PullPoint] HandleGrab");

        isGrabbed = true;

        // 손/컨트롤러 Transform 자동 감지
        grabbingInteractor = FindInteractorTransform();
    }

    // ============================
    //   EVENT: RELEASE UNSELECTED
    // ============================
    public void HandleRelease()
    {
        Debug.Log("[PullPoint] HandleRelease");

        isGrabbed = false;
        grabbingInteractor = null;

        ClearDots();

        float tension = Vector3.Distance(bandCenter.position, transform.position);
        if (tension >= minTension)
            PlanetShotEvents.InvokeOnRelease(tension, bandCenter.position, transform.position);
    }

    // ============================
    //  MAIN UPDATE LOOP
    // ============================
    private void Update()
    {
        if (isGrabbed && grabbingInteractor != null)
        {
            Vector3 dir = grabbingInteractor.position - bandCenter.position;
            float dist = Mathf.Min(dir.magnitude, maxStretch);

            transform.position = bandCenter.position + dir.normalized * dist;

            if (dist > minTension)
                ShowTrajectoryWithDots(dir.normalized * dist);
            else
                ClearDots();
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                restPosition.position,
                Time.deltaTime * returnSpeed
            );
            ClearDots();
        }
    }

    // ============================
    //  FIND INTERACTOR TRANSFORM
    // ============================
    private Transform FindInteractorTransform()
    {
        // 가장 가까운 Hand/Controller Interactor 자동 탐색
        // 필요하면 특정 이름/태그로 바꿔도 됨

        var interactors = FindObjectsOfType<MonoBehaviour>();

        float nearestDist = float.MaxValue;
        Transform nearest = null;

        foreach (var mb in interactors)
        {
            if (mb is IInteractorView)
            {
                Transform t = mb.transform;
                float d = Vector3.Distance(t.position, transform.position);

                if (d < nearestDist)
                {
                    nearestDist = d;
                    nearest = t;
                }
            }
        }

        return nearest;
    }

    // ============================
    //  TRAJECTORY DRAWING
    // ============================
    private void ShowTrajectoryWithDots(Vector3 pullVector)
    {
        ClearDots();

        float tension = pullVector.magnitude;
        if (tension < 0.01f) return;

        Vector3 launchDir = (bandCenter.position - transform.position).normalized;
        Vector3 launchVel = launchDir * tension * launchForceMultiplier;
        Vector3 startPos = transform.position;

        float velMag = Mathf.Max(launchVel.magnitude, 0.01f);
        int visiblePoints = Mathf.Min(maxPoints, Mathf.CeilToInt(velMag * 5f / pointSpacing));

        for (int i = 0; i < visiblePoints; i++)
        {
            float t = i * pointSpacing / velMag;
            
            Vector3 point = startPos + launchVel * t;

            GameObject dot = Instantiate(dotPrefab, point, Quaternion.identity);
            dot.transform.localScale = Vector3.one * pointScale;

            dots.Add(dot);
        }
    }

    private void ClearDots()
    {
        foreach (var d in dots)
            if (d != null) Destroy(d);

        dots.Clear();
    }
}
