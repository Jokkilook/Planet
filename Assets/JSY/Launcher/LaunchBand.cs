using UnityEngine;

public class LaunchBand : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform pullPoint;

    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 3;
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = Color.black;
    }

    void Update()
    {
        line.SetPosition(0, leftPoint.position);
        line.SetPosition(1, pullPoint.position);
        line.SetPosition(2, rightPoint.position);
    }
}
