using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class PullPointController : MonoBehaviour
{

    public Transform bandCenter;    
    public Transform restPosition;  
    public float maxStretch = 0.6f; 
    public float returnSpeed = 8f;  

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    bool isGrabbed = false;
    Transform grabbingInteractor;

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

        float tension = Vector3.Distance(bandCenter.position, transform.position);
        PlanetShotEvents.InvokeOnRelease(tension, bandCenter.position, transform.position);
    }

    void Update()
    {
        if (isGrabbed && grabbingInteractor != null)
        {
            Vector3 dir = grabbingInteractor.position - bandCenter.position;
            float dist = Mathf.Min(dir.magnitude, maxStretch);
            transform.position = bandCenter.position + dir.normalized * dist;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, restPosition.position, Time.deltaTime * returnSpeed);
        }
    }
}
