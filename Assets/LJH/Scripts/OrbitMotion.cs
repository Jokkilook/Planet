using UnityEngine;

public class OrbitMotion : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public Vector3 axis;

    public float orbitSpeed = 20.0f;

    void Update()
    {
        if (target != null)
        {
            transform.RotateAround(target.position, axis, orbitSpeed * Time.deltaTime);

            transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }
    }
}
