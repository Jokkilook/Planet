// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;
// using System.Collections.Generic;

// [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
// public class PullPointController : MonoBehaviour
// {
//     [Header("Band Settings")]
//     public Transform bandCenter;
//     public Transform restPosition;
//     public float maxStretch = 0.6f;
//     public float minTension = 0.1f;
//     public float returnSpeed = 8f;

//     [Header("Trajectory Settings")]
//     public int maxPoints = 50;               // 최대 점 개수
//     public float pointSpacing = 0.02f;       // 점 간 거리 (월드 단위)
//     public float pointScale = 0.02f;
//     public float launchForceMultiplier = 10f;
//     public GameObject dotPrefab;             // 작은 Sphere prefab
//     private List<GameObject> dots = new List<GameObject>();

//     private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
//     private bool isGrabbed = false;
//     private Transform grabbingInteractor;

//     void Awake()
//     {
//         grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
//         grab.selectEntered.AddListener(OnGrab);
//         grab.selectExited.AddListener(OnRelease);
//     }

//     void OnDestroy()
//     {
//         grab.selectEntered.RemoveListener(OnGrab);
//         grab.selectExited.RemoveListener(OnRelease);
//     }

//     void OnGrab(SelectEnterEventArgs args)
//     {
//         Debug.Log("Grabbed");
//         isGrabbed = true;
//         grabbingInteractor = args.interactorObject.transform;
//     }

//     void OnRelease(SelectExitEventArgs args)
//     {
//         Debug.Log("Released");
//         isGrabbed = false;
//         grabbingInteractor = null;
//         ClearDots();

//         float tension = Vector3.Distance(bandCenter.position, transform.position);
//         if (tension < minTension) return;

//         PlanetShotEvents.InvokeOnRelease(tension, bandCenter.position, transform.position);
//     }

//     void Update()
//     {
//         if (isGrabbed && grabbingInteractor != null)
//         {
//             Vector3 dir = grabbingInteractor.position - bandCenter.position;
//             float dist = Mathf.Min(dir.magnitude, maxStretch);
//             transform.position = bandCenter.position + dir.normalized * dist;

//             float tension = Vector3.Distance(bandCenter.position, transform.position);
//             if (tension > minTension)
//                 ShowTrajectoryWithDots(dir.normalized * dist);
//             else
//                 ClearDots();
//         }
//         else
//         {
//             transform.position = Vector3.Lerp(transform.position, restPosition.position, Time.deltaTime * returnSpeed);
//             ClearDots();
//         }
//     }

//     void ShowTrajectoryWithDots(Vector3 pullVector)
//     {
//         ClearDots();

//         float tension = pullVector.magnitude;
//         if (tension < 0.01f) return;

//         Vector3 launchDirection = (bandCenter.position - transform.position).normalized;
//         Vector3 launchVelocity = launchDirection * tension * launchForceMultiplier;
//         Vector3 startPosition = transform.position;

//         float velocityMag = launchVelocity.magnitude;
//         if (velocityMag < 0.01f) velocityMag = 0.01f; // 안전값

//         // 점 간격 기준으로 필요한 점 개수 계산
//         int visiblePoints = Mathf.Min(maxPoints, Mathf.CeilToInt(velocityMag * 5f / pointSpacing));

//         for (int i = 0; i < visiblePoints; i++)
//         {
//             float t = i * pointSpacing / velocityMag; // velocity에 맞춘 시간 간격
//             Vector3 point = startPosition + launchVelocity * t + 0.5f * Physics.gravity * t * t;

//             GameObject dot = Instantiate(dotPrefab, point, Quaternion.identity);
//             dot.transform.localScale = Vector3.one * pointScale;
//             dots.Add(dot);
//         }
//     }

//     void ClearDots()
//     {
//         foreach (var d in dots)
//             Destroy(d);
//         dots.Clear();
//     }
// }

// // /*
// // using UnityEngine;
// // using UnityEngine.XR.Interaction.Toolkit;
// // using UnityEngine.XR.Interaction.Toolkit.Interactables;
// // using UnityEngine.XR.Interaction.Toolkit.Interactors;
// // using System.Collections.Generic;

// // [RequireComponent(typeof(XRGrabInteractable))]
// // public class PullPointController : MonoBehaviour
// // {
// //     [Header("Band Settings")]
// //     public Transform bandCenter;
// //     public Transform restPosition;
// //     public float maxStretch = 0.6f;
// //     public float minTension = 0.1f;
// //     public float returnSpeed = 8f;

// //     [Header("Trajectory Settings")]
// //     public int maxPoints = 50;
// //     public float pointSpacing = 0.02f;
// //     public float pointScale = 0.02f;
// //     public float launchForceMultiplier = 10f;
// //     public GameObject dotPrefab;

// //     private List<GameObject> dots = new List<GameObject>();

// //     private XRGrabInteractable grab;
// //     private bool isGrabbed = false;

// //     // XR Interactor의 attachTransform (손/컨트롤러 위치)
// //     private Transform grabbingInteractor;

// //     void Awake()
// //     {
// //         grab = GetComponent<XRGrabInteractable>();

// //         grab.selectEntered.AddListener(OnGrab);
// //         grab.selectExited.AddListener(OnRelease);
// //     }

// //     void OnDestroy()
// //     {
// //         grab.selectEntered.RemoveListener(OnGrab);
// //         grab.selectExited.RemoveListener(OnRelease);
// //     }

// //     // -----------------------------
// //     //         ON GRAB
// //     // -----------------------------
// //     void OnGrab(SelectEnterEventArgs args)
// //     {
// //         isGrabbed = true;

// //         // XRIT 3.x : interactorObject는 인터페이스 기반이므로 transform 직접 사용 금지
// //         var interactor = args.interactorObject as IXRInteractor;

// //         if (interactor != null)
// //         {
// //             grabbingInteractor = interactor.GetAttachTransform(grab);
// //         }
// //         else
// //         {
// //             Debug.LogWarning("Interactor transform을 가져올 수 없습니다.");
// //         }
// //     }

// //     // -----------------------------
// //     //        ON RELEASE
// //     // -----------------------------
// //     void OnRelease(SelectExitEventArgs args)
// //     {
// //         isGrabbed = false;
// //         grabbingInteractor = null;
// //         ClearDots();

// //         float tension = Vector3.Distance(bandCenter.position, transform.position);
// //         if (tension < minTension) return;

// //         PlanetShotEvents.InvokeOnRelease(tension, bandCenter.position, transform.position);
// //     }

// //     // -----------------------------
// //     //            UPDATE
// //     // -----------------------------
// //     void Update()
// //     {
// //         if (isGrabbed && grabbingInteractor != null)
// //         {
// //             Vector3 dir = grabbingInteractor.position - bandCenter.position;
// //             float dist = Mathf.Min(dir.magnitude, maxStretch);

// //             transform.position = bandCenter.position + dir.normalized * dist;

// //             float tension = dist;
// //             if (tension > minTension)
// //                 ShowTrajectoryWithDots(dir.normalized * dist);
// //             else
// //                 ClearDots();
// //         }
// //         else
// //         {
// //             transform.position = Vector3.Lerp(transform.position, restPosition.position, Time.deltaTime * returnSpeed);
// //             ClearDots();
// //         }
// //     }

// //     // -----------------------------
// //     //     TRAJECTORY DOTS DRAW
// //     // -----------------------------
// //     void ShowTrajectoryWithDots(Vector3 pullVector)
// //     {
// //         ClearDots();

// //         float tension = pullVector.magnitude;
// //         if (tension < 0.01f) return;

// //         Vector3 launchDirection = (bandCenter.position - transform.position).normalized;
// //         Vector3 launchVelocity = launchDirection * tension * launchForceMultiplier;
// //         Vector3 startPosition = transform.position;

// //         float velocityMag = Mathf.Max(launchVelocity.magnitude, 0.01f);

// //         int visiblePoints = Mathf.Min(maxPoints, Mathf.CeilToInt(velocityMag * 5f / pointSpacing));

// //         for (int i = 0; i < visiblePoints; i++)
// //         {
// //             float t = i * pointSpacing / velocityMag;

// //             Vector3 point =
// //                 startPosition +
// //                 launchVelocity * t +
// //                 0.5f * Physics.gravity * t * t;

// //             GameObject dot = Instantiate(dotPrefab, point, Quaternion.identity);
// //             dot.transform.localScale = Vector3.one * pointScale;
// //             dots.Add(dot);
// //         }
// //     }

// //     // -----------------------------
// //     //        CLEAR DOTS
// //     // -----------------------------
// //     void ClearDots()
// //     {
// //         foreach (GameObject d in dots)
// //             if (d != null) Destroy(d);

// //         dots.Clear();
// //     }
// // }
// // */
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
            Vector3 point =
                startPos +
                launchVel * t +
                0.5f * Physics.gravity * t * t;

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
